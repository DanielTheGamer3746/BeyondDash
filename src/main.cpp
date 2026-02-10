#include <Geode/Geode.hpp>
#include <Geode/modify/MenuLayer.hpp>
#include <Geode/modify/LevelSelectLayer.hpp>
#include <Geode/binding/LevelSelectLayer.hpp>
#include <Geode/binding/LevelPage.hpp>
#include <Geode/binding/BoomScrollLayer.hpp>
#include <Geode/binding/LevelInfoLayer.hpp>
#include <Geode/binding/GJGameLevel.hpp>
#include <Geode/binding/ButtonSprite.hpp>

using namespace geode::prelude;

// =================================================================
// 1. STATE MANAGER
// =================================================================
class BeyondDashState {
public:
    static bool& isBeyondDash() {
        static bool active = false;
        return active;
    }
};

// =================================================================
// 2. DATA / LEVEL MANAGER
// =================================================================
struct LevelDef {
    int id;
    std::string name;
    int stars;
    int difficulty;
    bool isDemon;
    ccColor3B color;
};

class LevelManager {
public:
    static std::vector<LevelDef> getLevels() {
        return {
            { 104138684, "Explorers", 17, 60, true, {80, 40, 160} },
            { 102184114, "Rise", 10, 50, false, {144, 150, 35} },
        };
    }

    static GJGameLevel* createLevelObject(const LevelDef& def) {
        auto level = GJGameLevel::create();
        level->m_levelID = def.id;
        level->m_levelName = def.name;
        level->m_stars = def.stars;
        level->m_demon = def.isDemon;
        level->m_levelType = GJLevelType::Saved;
        level->m_difficulty = (GJDifficulty)(def.difficulty / 10);
        return level;
    }
};

// =================================================================
// 3. HOOK: MENU LAYER
// =================================================================
class $modify(BeyondDashMenu, MenuLayer) {

    void onBeyondDash(CCObject * sender) {
        BeyondDashState::isBeyondDash() = true;
        auto scene = CCScene::create();
        auto layer = LevelSelectLayer::create(0);
        scene->addChild(layer);
        CCDirector::sharedDirector()->replaceScene(CCTransitionFade::create(0.5f, scene));
    }

    bool init() {
        if (!MenuLayer::init()) return false;

        auto spr = ButtonSprite::create(
            "Beyond Dash", 0, false, "goldFont.fnt", "GJ_button_01.png", 0, 0.8f
        );

        auto btn = CCMenuItemSpriteExtra::create(
            spr, this, menu_selector(BeyondDashMenu::onBeyondDash)
        );

        auto menu = CCMenu::create();
        menu->addChild(btn);

        auto winSize = CCDirector::sharedDirector()->getWinSize();
        if (auto bottomMenu = this->getChildByID("bottom-menu")) {
            menu->setPosition(winSize.width / 2, bottomMenu->getPositionY() + 55);
        }
        else {
            menu->setPosition(winSize.width / 2, 100);
        }

        this->addChild(menu);
        return true;
    }
};

// =================================================================
// 4. HOOK: LEVEL SELECT LAYER
// =================================================================
class $modify(MyLevelSelectLayer, LevelSelectLayer) {
    struct Fields {
        CCSprite* m_bg = nullptr;
    };

    void onBack(CCObject * sender) {
        if (BeyondDashState::isBeyondDash()) {
            BeyondDashState::isBeyondDash() = false;
            CCDirector::sharedDirector()->replaceScene(
                CCTransitionFade::create(0.5f, MenuLayer::scene(false))
            );
        }
        else {
            LevelSelectLayer::onBack(sender);
        }
    }

    void onCustomPlay(CCObject * sender) {
        if (!m_scrollLayer) return;

        // FIXED: Use m_page (int) instead of getPage() (CCLayer*)
        int pageIndex = m_scrollLayer->m_page;

        auto levels = LevelManager::getLevels();

        // Handle wrapping manually just in case index is weird due to looping
        int count = levels.size();
        if (count == 0) return; // Safety check

        int safeIndex = (pageIndex % count + count) % count;

        if (safeIndex >= 0 && safeIndex < count) {
            auto levelObj = LevelManager::createLevelObject(levels[safeIndex]);
            auto layer = LevelInfoLayer::create(levelObj, false);
            CCDirector::sharedDirector()->getRunningScene()->addChild(layer);
        }
    }

    // Helper to update background color
    void updateCustomColor(float pagePosition) {
        if (!m_fields->m_bg) return;

        auto levels = LevelManager::getLevels();
        if (levels.empty()) return;

        int count = levels.size();

        // Floor the position to get the current index
        int index = std::floor(pagePosition);
        float percent = pagePosition - index;

        // Wrap indices to ensure they are valid
        int currentIdx = (index % count + count) % count;
        int nextIdx = ((index + 1) % count + count) % count;

        ccColor3B col1 = levels[currentIdx].color;
        ccColor3B col2 = levels[nextIdx].color;

        // Interpolate colors
        GLubyte r = col1.r + (GLubyte)((col2.r - col1.r) * percent);
        GLubyte g = col1.g + (GLubyte)((col2.g - col1.g) * percent);
        GLubyte b = col1.b + (GLubyte)((col2.b - col1.b) * percent);

        m_fields->m_bg->setColor({ r, g, b });
    }

    // Hook scroll movement to update color
    void scrollLayerMoved(CCPoint p0) {
        LevelSelectLayer::scrollLayerMoved(p0);

        if (BeyondDashState::isBeyondDash() && m_scrollLayer) {
            auto winSize = CCDirector::sharedDirector()->getWinSize();
            // Calculate which page we are visually on (float)
            float page = -m_scrollLayer->getPositionX() / winSize.width;
            this->updateCustomColor(page);
        }
    }

    bool init(int page) {
        if (!BeyondDashState::isBeyondDash()) {
            return LevelSelectLayer::init(page);
        }

        if (!CCLayer::init()) return false;

        auto winSize = CCDirector::sharedDirector()->getWinSize();

        // 1. Background
        auto bg = CCSprite::create("game_bg_01_001.png");
        bg->setAnchorPoint({ 0.5f, 0.5f });
        bg->setPosition(winSize / 2);
        bg->setScaleX((winSize.width + 10) / bg->getTextureRect().size.width);
        bg->setScaleY((winSize.height + 10) / bg->getTextureRect().size.height);
        bg->setColor({ 0, 0, 0 });
        this->addChild(bg, -2);
        m_fields->m_bg = bg;

        // 2. Prepare Pages
        auto pages = CCArray::create();
        auto customLevels = LevelManager::getLevels();

        for (const auto& data : customLevels) {
            auto level = LevelManager::createLevelObject(data);
            auto pageLayer = LevelPage::create(level);
            pageLayer->updateDynamicPage(level);

            // Hook the play button on the page
            if (auto menu = pageLayer->getChildByType<CCMenu>(0)) {
                if (auto btn = menu->getChildByType<CCMenuItemSpriteExtra>(0)) {
                    btn->setTarget(this, menu_selector(MyLevelSelectLayer::onCustomPlay));
                }
            }
            pages->addObject(pageLayer);
        }

        // 3. Create BoomScrollLayer
        // FIXED: Removed duplicate declaration here
        auto boomScroll = BoomScrollLayer::create(
            pages,
            0,
            true,
            nullptr,
            this
        );

        boomScroll->setPosition(0, 0);

        this->addChild(boomScroll, 10);
        this->m_scrollLayer = boomScroll;

        // 4. Update
        boomScroll->updatePages();
        boomScroll->moveToPage(0);

        // Initial color update
        this->updateCustomColor(0.0f);

        // 5. Back Button
        auto backSprite = CCSprite::createWithSpriteFrameName("GJ_arrow_01_001.png");
        auto backBtn = CCMenuItemSpriteExtra::create(
            backSprite, this, menu_selector(MyLevelSelectLayer::onBack)
        );
        auto menu = CCMenu::create();
        menu->addChild(backBtn);
        menu->setPosition(25, winSize.height - 25);
        this->addChild(menu, 20);

        this->setKeypadEnabled(true);
        this->setTouchEnabled(true);

        return true;
    }
};
#include <Geode/Geode.hpp>
#include <Geode/modify/MenuLayer.hpp>
#include <Geode/binding/LevelSelectLayer.hpp>
#include <Geode/binding/LevelPage.hpp>
#include <Geode/binding/BoomScrollLayer.hpp>
#include <Geode/binding/DynamicScrollDelegate.hpp>
#include <Geode/binding/LevelInfoLayer.hpp>
#include <Geode/binding/GJGameLevel.hpp>
#include <Geode/binding/ButtonSprite.hpp>
#include <Geode/binding/GameManager.hpp>
#include <Geode/binding/MusicDownloadManager.hpp>
#include <Geode/binding/SongInfoObject.hpp>
#include <Geode/binding/PlayLayer.hpp>
#include <Geode/binding/GameLevelManager.hpp>
#include <Geode/binding/LevelDownloadDelegate.hpp>
#include <Geode/binding/MusicDownloadDelegate.hpp>
#include <Geode/binding/LoadingCircle.hpp>
#include <algorithm>

using namespace geode::prelude;

// =================================================================
// 1. DATA & DEFINITIONS
// =================================================================

struct LevelDef {
    int id;
    std::string name;
    int stars;
    int difficulty;
    bool isDemon;
    std::vector<int> songIDs;
    ccColor3B color;
};

class LevelManager {
public:
    static const std::vector<LevelDef>& getLevels() {
        static std::vector<LevelDef> levels = {
            { 104138684, "Explorers", 17, 60, true, {1183462}, {80, 40, 160} },
            { 102184114, "Rise", 10, 50, false, {10007245, 10006699, 10004345}, {144, 150, 35} },
            { 110790559, "Theory of Everything 3", 15, 60, true, {738567}, {164, 21, 21} },
        };
        return levels;
    }

    static GJGameLevel* createLevelObject(const LevelDef& def) {
        auto level = GJGameLevel::create();
        level->m_levelID = def.id;
        level->m_levelName = def.name;
        level->m_stars = def.stars;
        level->m_demon = def.isDemon;
        level->m_levelType = GJLevelType::Saved;
        level->m_difficulty = (GJDifficulty)(def.difficulty / 10);

        if (!def.songIDs.empty()) {
            level->m_audioTrack = def.songIDs[0];
        }

        if (auto savedLevel = GameLevelManager::sharedState()->getSavedLevel(def.id)) {
            level->m_normalPercent = savedLevel->m_normalPercent;
            level->m_practicePercent = savedLevel->m_practicePercent;
            level->m_orbCompletion = savedLevel->m_orbCompletion;
            level->m_newNormalPercent2 = savedLevel->m_newNormalPercent2;
            level->m_coins = savedLevel->m_coins;
        }

        return level;
    }
};

// =================================================================
// 2. LEVEL LAUNCHER
// =================================================================

class LevelLauncher : public CCObject, public LevelDownloadDelegate, public MusicDownloadDelegate {
    int m_levelID;
    int m_songID;
    LoadingCircle* m_loadingCircle = nullptr;

public:
    static LevelLauncher* create(int levelID, int songID) {
        auto ret = new LevelLauncher();
        ret->m_levelID = levelID;
        ret->m_songID = songID;
        ret->autorelease();
        return ret;
    }

    ~LevelLauncher() {
        auto glm = GameLevelManager::sharedState();
        if (glm->m_levelDownloadDelegate == this) {
            glm->m_levelDownloadDelegate = nullptr;
        }
    }

    void start() {
        auto scene = CCDirector::sharedDirector()->getRunningScene();
        m_loadingCircle = LoadingCircle::create();
        m_loadingCircle->setParentLayer(nullptr);
        m_loadingCircle->setPosition(CCDirector::sharedDirector()->getWinSize() / 2);

        if (scene) {
            scene->addChild(m_loadingCircle, 999);
            m_loadingCircle->show();
        }

        auto glm = GameLevelManager::sharedState();
        auto existingLevel = glm->getSavedLevel(m_levelID);

        if (existingLevel && std::string_view(existingLevel->m_levelString).length() > 0) {
            this->checkAudioAndPlay(existingLevel);
        }
        else {
            glm->m_levelDownloadDelegate = this;
            glm->downloadLevel(m_levelID, false, false);
        }
    }

    void levelDownloadFinished(GJGameLevel* level) override {
        GameLevelManager::sharedState()->m_levelDownloadDelegate = nullptr;
        checkAudioAndPlay(level);
    }

    void levelDownloadFailed(int levelID) override {
        GameLevelManager::sharedState()->m_levelDownloadDelegate = nullptr;
        if (m_loadingCircle) m_loadingCircle->fadeAndRemove();
        Notification::create("Failed to download level", NotificationIcon::Error)->show();
    }

    void checkAudioAndPlay(GJGameLevel* level) {
        int songID = level->m_audioTrack;
        if (songID == 0) songID = m_songID;

        auto mdm = MusicDownloadManager::sharedState();

        if (songID == 0 || mdm->isSongDownloaded(songID)) {
            enterLevel(level);
        }
        else {
            mdm->downloadSong(songID);
            enterLevel(level);
        }
    }

    void downloadSongFinished(int songID) override {
        auto level = GameLevelManager::sharedState()->getSavedLevel(m_levelID);
        if (level) enterLevel(level);
    }

    void downloadSongFailed(int songID, GJSongError error) override {
        if (m_loadingCircle) m_loadingCircle->fadeAndRemove();
        Notification::create("Failed to download song", NotificationIcon::Error)->show();
    }

    void loadSongInfoFinished(SongInfoObject*) override {}
    void loadSongInfoFailed(int, GJSongError) override {}

    void enterLevel(GJGameLevel* level) {
        if (m_loadingCircle) {
            m_loadingCircle->fadeAndRemove();
            m_loadingCircle = nullptr;
        }

        auto scene = CCScene::create();
        auto playLayer = PlayLayer::create(level, false, false);
        scene->addChild(playLayer);
        CCDirector::sharedDirector()->replaceScene(CCTransitionFade::create(0.5f, scene));
    }
};

// =================================================================
// 3. UI POPUPS
// =================================================================

// Replaced geode::Popup with the classic FLAlertLayer for maximum compatibility
class SongListPopup : public FLAlertLayer {
protected:
    LevelDef m_level;

    bool init(LevelDef const& level) {
        m_level = level;

        // Initialize the base alert layer with a dim background (150 opacity)
        if (!FLAlertLayer::init(150)) return false;

        auto winSize = CCDirector::sharedDirector()->getWinSize();

        // Create the popup background
        auto bg = CCScale9Sprite::create("GJ_square01.png", { 0, 0, 80, 80 });
        float width = 260.0f;
        float height = std::max(160.0f, 80.0f + (level.songIDs.size() * 35.0f));
        bg->setContentSize({ width, height });
        bg->setPosition(winSize / 2);
        this->m_mainLayer->addChild(bg, -1);

        // Title
        auto title = CCLabelBMFont::create("Level Audio", "goldFont.fnt");
        title->setPosition({ winSize.width / 2, winSize.height / 2 + height / 2 - 20 });
        title->setScale(0.7f);
        this->m_mainLayer->addChild(title);

        // Close Button
        auto closeBtnSpr = CCSprite::createWithSpriteFrameName("GJ_closeBtn_001.png");
        auto closeBtn = CCMenuItemSpriteExtra::create(closeBtnSpr, this, menu_selector(SongListPopup::onClose));

        auto closeMenu = CCMenu::create();
        closeMenu->setPosition({ winSize.width / 2 - width / 2 + 5, winSize.height / 2 + height / 2 - 5 });
        closeMenu->addChild(closeBtn);
        this->m_mainLayer->addChild(closeMenu);

        // Main content menu
        auto menu = CCMenu::create();
        menu->setPosition(winSize.width / 2, winSize.height / 2 - 15);
        this->m_mainLayer->addChild(menu);

        if (level.songIDs.empty()) {
            auto label = CCLabelBMFont::create("No custom songs.", "bigFont.fnt");
            label->setScale(0.5f);
            menu->addChild(label);
            return true;
        }

        float startY = (level.songIDs.size() * 35.0f) / 2.0f - 17.5f;

        for (size_t i = 0; i < level.songIDs.size(); ++i) {
            int songID = level.songIDs[i];

            auto label = CCLabelBMFont::create(fmt::format("Song ID: {}", songID).c_str(), "bigFont.fnt");
            // Slightly reduced scale so long IDs don't get too wide
            label->setScale(0.45f);
            label->setAnchorPoint({ 0.0f, 0.5f });
            // Moved further to the left edge of the popup
            label->setPosition({ -115.0f, startY - (i * 35.0f) });
            menu->addChild(label);

            auto dlBtnSpr = ButtonSprite::create("Get", 30, true, "goldFont.fnt", "GJ_button_01.png", 30, 0.6f);
            auto dlBtn = CCMenuItemSpriteExtra::create(dlBtnSpr, this, menu_selector(SongListPopup::onDownloadSong));
            // Moved further to the right edge of the popup
            dlBtn->setPosition({ 85.0f, startY - (i * 35.0f) });
            dlBtn->setTag(songID);
            menu->addChild(dlBtn);
        }

        this->setKeypadEnabled(true);
        this->setTouchEnabled(true);
        return true;
    }

    void onClose(CCObject*) {
        this->setKeyboardEnabled(false);
        this->removeFromParentAndCleanup(true);
    }

    void keyBackClicked() override {
        this->onClose(nullptr);
    }

    void onDownloadSong(CCObject* sender) {
        int songID = sender->getTag();
        auto mdm = MusicDownloadManager::sharedState();

        if (mdm->isSongDownloaded(songID)) {
            Notification::create("Song already downloaded!", NotificationIcon::Success)->show();
        }
        else {
            mdm->downloadSong(songID);
            Notification::create(fmt::format("Triggered download for {}", songID), NotificationIcon::Loading)->show();
        }
    }

public:
    static SongListPopup* create(LevelDef const& level) {
        auto ret = new SongListPopup();
        if (ret && ret->init(level)) {
            ret->autorelease();
            return ret;
        }
        CC_SAFE_DELETE(ret);
        return nullptr;
    }
};

// =================================================================
// 4. CUSTOM LAYER
// =================================================================

class BeyondDashLayer : public CCLayer, public DynamicScrollDelegate {
protected:
    std::vector<LevelDef> m_levels;
    BoomScrollLayer* m_scrollLayer = nullptr;
    CCSprite* m_bg = nullptr;
    Ref<LevelLauncher> m_activeLauncher;

public:
    static BeyondDashLayer* create() {
        auto ret = new BeyondDashLayer();
        if (ret && ret->init()) {
            ret->autorelease();
            return ret;
        }
        CC_SAFE_DELETE(ret);
        return nullptr;
    }

    static CCScene* scene() {
        auto scene = CCScene::create();
        auto layer = BeyondDashLayer::create();
        scene->addChild(layer);
        return scene;
    }

    // Reverted to your original logic that compiled correctly
    void update(float dt) override {
        if (m_scrollLayer) {
            this->scrollLayerMoved(m_scrollLayer->getPosition());
        }
    }

    void scrollLayerMoved(cocos2d::CCPoint p0) {
        if (m_scrollLayer) {
            auto winSize = CCDirector::sharedDirector()->getWinSize();
            float page = -p0.x / winSize.width;
            this->updateCustomColor(page);
        }
    }

    void updatePageWithObject(CCObject* o, CCObject* object) override {}

    void onBack(CCObject*) {
        CCDirector::sharedDirector()->replaceScene(
            CCTransitionFade::create(0.5f, MenuLayer::scene(false))
        );
    }

    void onPrev(CCObject*) {
        if (!m_scrollLayer) return;
        int page = m_scrollLayer->m_page - 1;
        if (page < 0) page = m_levels.size() - 1;
        m_scrollLayer->moveToPage(page);
    }

    void onNext(CCObject*) {
        if (!m_scrollLayer) return;
        int page = m_scrollLayer->m_page + 1;
        if (page >= m_levels.size()) page = 0;
        m_scrollLayer->moveToPage(page);
    }

    void onPlayLevel(CCObject* sender) {
        auto btn = static_cast<CCNode*>(sender);
        int index = btn->getTag();
        if (index < 0 || index >= m_levels.size()) return;

        LevelDef def = m_levels[index];
        int songID = def.songIDs.empty() ? 0 : def.songIDs[0];

        m_activeLauncher = LevelLauncher::create(def.id, songID);
        m_activeLauncher->start();
    }

    void onOpenMusicPopup(CCObject* sender) {
        auto btn = static_cast<CCNode*>(sender);
        int index = btn->getTag();
        if (index < 0 || index >= m_levels.size()) return;

        LevelDef def = m_levels[index];
        SongListPopup::create(def)->show();
    }

    void updateCustomColor(float pagePosition) {
        if (!m_bg || m_levels.empty()) return;

        int count = m_levels.size();
        auto safeMod = [](int n, int m) { return ((n % m) + m) % m; };

        if (pagePosition < 0) pagePosition = 0;
        if (pagePosition > count - 1) pagePosition = count - 1;

        int index = std::floor(pagePosition);
        float percent = pagePosition - index;

        int currentIdx = safeMod(index, count);
        int nextIdx = safeMod(index + 1, count);

        ccColor3B col1 = m_levels[currentIdx].color;
        ccColor3B col2 = m_levels[nextIdx].color;

        GLubyte r = col1.r + (GLubyte)((col2.r - col1.r) * percent);
        GLubyte g = col1.g + (GLubyte)((col2.g - col1.g) * percent);
        GLubyte b = col1.b + (GLubyte)((col2.b - col1.b) * percent);

        m_bg->setColor({ r, g, b });
    }

    bool init() override {
        if (!CCLayer::init()) return false;

        m_levels = LevelManager::getLevels();
        auto winSize = CCDirector::sharedDirector()->getWinSize();

        auto bg = CCSprite::create("game_bg_01_001.png");
        bg->setPosition(winSize / 2);

        if (bg->getContentSize().width > 0) {
            float scaleX = winSize.width / bg->getContentSize().width;
            float scaleY = winSize.height / bg->getContentSize().height;
            bg->setScale(std::max(scaleX, scaleY));
        }

        bg->setColor({ 0, 0, 0 });
        this->addChild(bg, -2);
        m_bg = bg;

        auto pages = CCArray::create();
        int idx = 0;
        this->scheduleUpdate();

        for (const auto& data : m_levels) {
            auto level = LevelManager::createLevelObject(data);
            auto pageLayer = LevelPage::create(level);
            pageLayer->updateDynamicPage(level);

            if (auto menu = pageLayer->getChildByType<CCMenu>(0)) {
                // Hook up the existing play button
                if (auto btn = menu->getChildByType<CCMenuItemSpriteExtra>(0)) {
                    btn->setTarget(this, menu_selector(BeyondDashLayer::onPlayLevel));
                    btn->setTag(idx);
                }

                // Add the new "Music" button
                auto musicBtnSpr = ButtonSprite::create("Music", 0, false, "goldFont.fnt", "GJ_button_02.png", 0, 0.6f);
                auto musicBtn = CCMenuItemSpriteExtra::create(musicBtnSpr, this, menu_selector(BeyondDashLayer::onOpenMusicPopup));
                musicBtn->setTag(idx);

                // Position it in the bottom right corner of the screen
                float btnX = (winSize.width / 2) - 55.0f;
                float btnY = -(winSize.height / 2) + 35.0f;
                musicBtn->setPosition({ btnX, btnY });

                menu->addChild(musicBtn);
            }
            pages->addObject(pageLayer);
            idx++;
        }

        auto boomScroll = BoomScrollLayer::create(pages, 0, true, pages, this);
        this->addChild(boomScroll, 10);
        m_scrollLayer = boomScroll;
        boomScroll->updatePages();
        boomScroll->moveToPage(0);

        this->updateCustomColor(0.0f);

        auto uiMenu = CCMenu::create();

        auto backSprite = CCSprite::createWithSpriteFrameName("GJ_arrow_01_001.png");
        auto backBtn = CCMenuItemSpriteExtra::create(
            backSprite, this, menu_selector(BeyondDashLayer::onBack)
        );
        backBtn->setPosition(-winSize.width / 2 + 25, winSize.height / 2 - 25);
        uiMenu->addChild(backBtn);

        auto leftSpr = CCSprite::createWithSpriteFrameName("GJ_arrow_03_001.png");
        auto leftBtn = CCMenuItemSpriteExtra::create(
            leftSpr, this, menu_selector(BeyondDashLayer::onPrev)
        );
        leftBtn->setPosition(-winSize.width / 2 + 30, 0);
        uiMenu->addChild(leftBtn);

        auto rightSpr = CCSprite::createWithSpriteFrameName("GJ_arrow_03_001.png");
        rightSpr->setFlipX(true);
        auto rightBtn = CCMenuItemSpriteExtra::create(
            rightSpr, this, menu_selector(BeyondDashLayer::onNext)
        );
        rightBtn->setPosition(winSize.width / 2 - 30, 0);
        uiMenu->addChild(rightBtn);

        this->addChild(uiMenu, 20);

        this->setKeypadEnabled(true);
        this->setTouchEnabled(true);

        return true;
    }

    void keyBackClicked() override {
        this->onBack(nullptr);
    }
};

// =================================================================
// 5. HOOK: MENU LAYER (Entry Point)
// =================================================================

class $modify(BeyondDashMenu, MenuLayer) {
    void onBeyondDash(CCObject * sender) {
        CCDirector::sharedDirector()->replaceScene(
            CCTransitionFade::create(0.5f, BeyondDashLayer::scene())
        );
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
        menu->setID("beyond-dash-menu"_spr);

        auto winSize = CCDirector::sharedDirector()->getWinSize();
        if (auto bottomMenu = this->getChildByID("bottom-menu")) {
            menu->setPosition(winSize.width / 2, bottomMenu->getPositionY() + 60);
        }
        else {
            menu->setPosition(winSize.width / 2, 100);
        }

        this->addChild(menu);
        return true;
    }
};
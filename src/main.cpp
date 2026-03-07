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

struct LevelDef {
    int id;
    std::string name;
    int stars;
    int difficulty;
    bool isDemon;
    std::vector<int> songIDs;
    ccColor3B color;
};

// Global level data
namespace LevelData {
    const std::vector<LevelDef>& get() {
        static std::vector<LevelDef> levels = {
            { 104138684, "Explorers", 17, 60, true, {1183462}, {80, 40, 160} },
            { 102184114, "Rise", 10, 50, false, {10007245, 10006699, 10004345}, {144, 150, 35} },
            { 110790559, "Theory of Everything 3", 15, 60, true, {738567}, {164, 21, 21} },
        };
        return levels;
    }

    GJGameLevel* buildGameLevel(const LevelDef& def) {
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

        // restore user progress if it exists
        if (auto saved = GameLevelManager::sharedState()->getSavedLevel(def.id)) {
            level->m_normalPercent = saved->m_normalPercent;
            level->m_practicePercent = saved->m_practicePercent;
            level->m_orbCompletion = saved->m_orbCompletion;
            level->m_newNormalPercent2 = saved->m_newNormalPercent2;
            level->m_coins = saved->m_coins;
        }

        return level;
    }
}

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
        auto scene = CCDirector::get()->getRunningScene();
        m_loadingCircle = LoadingCircle::create();
        m_loadingCircle->setParentLayer(nullptr);
        m_loadingCircle->setPosition(CCDirector::get()->getWinSize() / 2);

        if (scene) {
            scene->addChild(m_loadingCircle, 999);
            m_loadingCircle->show();
        }

        auto glm = GameLevelManager::sharedState();
        auto existing = glm->getSavedLevel(m_levelID);

        if (existing && std::string_view(existing->m_levelString).length() > 0) {
            verifyAudioAndPlay(existing);
        }
        else {
            glm->m_levelDownloadDelegate = this;
            glm->downloadLevel(m_levelID, false, false);
        }
    }

    void levelDownloadFinished(GJGameLevel* level) override {
        GameLevelManager::sharedState()->m_levelDownloadDelegate = nullptr;
        verifyAudioAndPlay(level);
    }

    void levelDownloadFailed(int levelID) override {
        GameLevelManager::sharedState()->m_levelDownloadDelegate = nullptr;
        if (m_loadingCircle) m_loadingCircle->fadeAndRemove();
        Notification::create("Download failed", NotificationIcon::Error)->show();
    }

    void verifyAudioAndPlay(GJGameLevel* level) {
        int songID = level->m_audioTrack == 0 ? m_songID : level->m_audioTrack;
        auto mdm = MusicDownloadManager::sharedState();

        if (songID == 0 || mdm->isSongDownloaded(songID)) {
            launchLevel(level);
        }
        else {
            mdm->downloadSong(songID);
            launchLevel(level); // todo: maybe wait for download to finish first?
        }
    }

    void downloadSongFinished(int songID) override {
        auto level = GameLevelManager::sharedState()->getSavedLevel(m_levelID);
        if (level) launchLevel(level);
    }

    void downloadSongFailed(int songID, GJSongError error) override {
        if (m_loadingCircle) m_loadingCircle->fadeAndRemove();
        Notification::create("Song download failed", NotificationIcon::Error)->show();
    }

    void loadSongInfoFinished(SongInfoObject*) override {}
    void loadSongInfoFailed(int, GJSongError) override {}

    void launchLevel(GJGameLevel* level) {
        if (m_loadingCircle) {
            m_loadingCircle->fadeAndRemove();
            m_loadingCircle = nullptr;
        }

        auto scene = CCScene::create();
        scene->addChild(PlayLayer::create(level, false, false));
        CCDirector::get()->replaceScene(CCTransitionFade::create(0.5f, scene));
    }
};

class SongListPopup : public FLAlertLayer {
protected:
    LevelDef m_level;

    bool init(LevelDef const& level) {
        m_level = level;
        if (!FLAlertLayer::init(150)) return false;

        auto winSize = CCDirector::get()->getWinSize();

        float width = 260.0f;
        float height = std::max(160.0f, 80.0f + (level.songIDs.size() * 35.0f));

        auto bg = CCScale9Sprite::create("GJ_square01.png", { 0, 0, 80, 80 });
        bg->setContentSize({ width, height });
        bg->setPosition(winSize / 2);
        m_mainLayer->addChild(bg, -1);

        auto title = CCLabelBMFont::create("Level Audio", "goldFont.fnt");
        title->setPosition({ winSize.width / 2, winSize.height / 2 + height / 2 - 20 });
        title->setScale(0.7f);
        m_mainLayer->addChild(title);

        auto closeBtn = CCMenuItemSpriteExtra::create(
            CCSprite::createWithSpriteFrameName("GJ_closeBtn_001.png"),
            this, menu_selector(SongListPopup::onClose)
        );

        auto closeMenu = CCMenu::create();
        closeMenu->setPosition({ winSize.width / 2 - width / 2 + 5, winSize.height / 2 + height / 2 - 5 });
        closeMenu->addChild(closeBtn);
        m_mainLayer->addChild(closeMenu);

        auto menu = CCMenu::create();
        menu->setPosition(winSize.width / 2, winSize.height / 2 - 15);
        m_mainLayer->addChild(menu);

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
            label->setScale(0.45f);
            label->setAnchorPoint({ 0.0f, 0.5f });
            label->setPosition({ -115.0f, startY - (i * 35.0f) });
            menu->addChild(label);

            auto dlBtn = CCMenuItemSpriteExtra::create(
                ButtonSprite::create("Get", 30, true, "goldFont.fnt", "GJ_button_01.png", 30, 0.6f),
                this, menu_selector(SongListPopup::onDownloadSong)
            );
            dlBtn->setPosition({ 85.0f, startY - (i * 35.0f) });
            dlBtn->setTag(songID);
            menu->addChild(dlBtn);
        }

        setKeypadEnabled(true);
        setTouchEnabled(true);
        return true;
    }

    void onClose(CCObject*) {
        setKeyboardEnabled(false);
        removeFromParentAndCleanup(true);
    }

    void keyBackClicked() override {
        onClose(nullptr);
    }

    void onDownloadSong(CCObject* sender) {
        int songID = sender->getTag();
        auto mdm = MusicDownloadManager::sharedState();

        if (mdm->isSongDownloaded(songID)) {
            Notification::create("Already downloaded!", NotificationIcon::Success)->show();
        }
        else {
            mdm->downloadSong(songID);
            Notification::create(fmt::format("Downloading {}", songID), NotificationIcon::Loading)->show();
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
        scene->addChild(BeyondDashLayer::create());
        return scene;
    }

    void update(float dt) override {
        if (m_scrollLayer) {
            scrollLayerMoved(m_scrollLayer->getPosition());
        }
    }

    void scrollLayerMoved(cocos2d::CCPoint p0) {
        if (m_scrollLayer) {
            auto winSize = CCDirector::get()->getWinSize();
            updateCustomColor(-p0.x / winSize.width);
        }
    }

    void updatePageWithObject(CCObject* o, CCObject* object) override {}

    void onBack(CCObject*) {
        CCDirector::get()->replaceScene(CCTransitionFade::create(0.5f, MenuLayer::scene(false)));
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
        int index = sender->getTag();
        if (index < 0 || index >= m_levels.size()) return;

        auto def = m_levels[index];
        int songID = def.songIDs.empty() ? 0 : def.songIDs[0];

        m_activeLauncher = LevelLauncher::create(def.id, songID);
        m_activeLauncher->start();
    }

    void onOpenMusicPopup(CCObject* sender) {
        int index = sender->getTag();
        if (index >= 0 && index < m_levels.size()) {
            SongListPopup::create(m_levels[index])->show();
        }
    }

    void updateCustomColor(float pagePosition) {
        if (!m_bg || m_levels.empty()) return;

        int count = m_levels.size();

        // clamp bounds
        pagePosition = std::clamp(pagePosition, 0.0f, (float)(count - 1));

        int index = std::floor(pagePosition);
        float percent = pagePosition - index;

        int currentIdx = index % count;
        int nextIdx = (index + 1) % count;

        ccColor3B col1 = m_levels[currentIdx].color;
        ccColor3B col2 = m_levels[nextIdx].color;

        m_bg->setColor({
            (GLubyte)(col1.r + (col2.r - col1.r) * percent),
            (GLubyte)(col1.g + (col2.g - col1.g) * percent),
            (GLubyte)(col1.b + (col2.b - col1.b) * percent)
            });
    }

    bool init() override {
        if (!CCLayer::init()) return false;

        m_levels = LevelData::get();
        auto winSize = CCDirector::get()->getWinSize();

        m_bg = CCSprite::create("game_bg_01_001.png");
        m_bg->setPosition(winSize / 2);

        if (m_bg->getContentSize().width > 0) {
            float scaleX = winSize.width / m_bg->getContentSize().width;
            float scaleY = winSize.height / m_bg->getContentSize().height;
            m_bg->setScale(std::max(scaleX, scaleY));
        }

        m_bg->setColor({ 0, 0, 0 });
        addChild(m_bg, -2);

        auto pages = CCArray::create();
        int idx = 0;
        scheduleUpdate();

        for (const auto& data : m_levels) {
            auto level = LevelData::buildGameLevel(data);
            auto pageLayer = LevelPage::create(level);
            pageLayer->updateDynamicPage(level);

            if (auto menu = pageLayer->getChildByType<CCMenu>(0)) {
                // hijack the play button
                if (auto btn = menu->getChildByType<CCMenuItemSpriteExtra>(0)) {
                    btn->setTarget(this, menu_selector(BeyondDashLayer::onPlayLevel));
                    btn->setTag(idx);
                }

                // add music button to the bottom leftish
                auto musicBtn = CCMenuItemSpriteExtra::create(
                    ButtonSprite::create("Music", 0, false, "goldFont.fnt", "GJ_button_02.png", 0, 0.6f),
                    this, menu_selector(BeyondDashLayer::onOpenMusicPopup)
                );
                musicBtn->setTag(idx);
                musicBtn->setPosition({ (winSize.width / 2) - 55.0f, -(winSize.height / 2) + 35.0f });
                menu->addChild(musicBtn);
            }
            pages->addObject(pageLayer);
            idx++;
        }

        m_scrollLayer = BoomScrollLayer::create(pages, 0, true, pages, this);
        addChild(m_scrollLayer, 10);
        m_scrollLayer->updatePages();
        m_scrollLayer->moveToPage(0);

        updateCustomColor(0.0f);

        // ui setup
        auto uiMenu = CCMenu::create();

        auto backBtn = CCMenuItemSpriteExtra::create(
            CCSprite::createWithSpriteFrameName("GJ_arrow_01_001.png"),
            this, menu_selector(BeyondDashLayer::onBack)
        );
        backBtn->setPosition(-winSize.width / 2 + 25, winSize.height / 2 - 25);
        uiMenu->addChild(backBtn);

        auto leftBtn = CCMenuItemSpriteExtra::create(
            CCSprite::createWithSpriteFrameName("GJ_arrow_03_001.png"),
            this, menu_selector(BeyondDashLayer::onPrev)
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

        addChild(uiMenu, 20);

        setKeypadEnabled(true);
        setTouchEnabled(true);

        return true;
    }

    void keyBackClicked() override {
        onBack(nullptr);
    }
};

class $modify(BeyondDashMenu, MenuLayer) {
    void onBeyondDash(CCObject * sender) {
        CCDirector::get()->replaceScene(CCTransitionFade::create(0.5f, BeyondDashLayer::scene()));
    }

    bool init() {
        if (!MenuLayer::init()) return false;

        auto btn = CCMenuItemSpriteExtra::create(
            ButtonSprite::create("Beyond Dash", 0, false, "goldFont.fnt", "GJ_button_01.png", 0, 0.8f),
            this, menu_selector(BeyondDashMenu::onBeyondDash)
        );

        auto menu = CCMenu::create();
        menu->addChild(btn);
        menu->setID("beyond-dash-menu"_spr);

        auto winSize = CCDirector::get()->getWinSize();
        if (auto bottomMenu = getChildByID("bottom-menu")) {
            menu->setPosition(winSize.width / 2, bottomMenu->getPositionY() + 60);
        }
        else {
            menu->setPosition(winSize.width / 2, 100);
        }

        addChild(menu);
        return true;
    }
};
using iTextSharp.tool.xml.html;
using ServiceStack;

< !DOCTYPE html >
< HTML Lang = "en" >
< head >
    < meta charset = "UTF-8" >
    < meta name = "viewport" content = "width=device-width, initial-scale=1.0, user-scalable=no" >
    < title > Directional Defender </ title >
    < link href = "https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;800&display=swap" rel = "stylesheet" >
    < link href = "https://fonts.googleapis.com/css2?family=Press+Start+2P&display=swap" rel = "stylesheet" >
    < link href = "https://fonts.googleapis.com/css2?family=Rubik:wght@400;600;800&display=swap" rel = "stylesheet" >
    < link href = "https://fonts.googleapis.com/css2?family=Luckiest+Guy&display=swap" rel = "stylesheet" >
    < style >
        body {
margin: 0;
padding: 0;
    background - color: #121212;
            color: #ffffff;
            font - family: 'Poppins', sans - serif;
overflow: hidden;
display: flex;
    justify - content: center;
    align - items: center;
height: 100vh;
    touch - action: none;
}
canvas {
            display: block;
background - color: #1a1a1a;
            border - radius: 16px;
box - shadow: 0 0 30px rgba(0, 255, 255, 0.2);
transition: background - color 0.3s ease-in-out;
z - index: 0;
        }
        .ui - container {
position: absolute;
top: 0;
left: 0;
width: 100 %;
height: 100 %;
display: flex;
    flex - direction: column;
    justify - content: center;
    align - items: center;
    pointer - events: none;
    text - align: center;
    z - index: 10;
}
        .screen {
            background-color: rgba(0, 0, 0, 0.7);
padding: 25px 40px;
border - radius: 20px;
pointer - events: all;
backdrop - filter: blur(10px);
border: 1px solid rgba(255, 255, 255, 0.1);
display: flex;
flex - direction: column;
align - items: center;
gap: 12px;
max - height: 95vh;
box - sizing: border - box;
        }
        .hidden { display: none!important; }
        h1 {
            font-size: 3.5rem;
margin: 0;
margin - bottom: 10px;
font - weight: 800;
text - transform: uppercase;
letter - spacing: 2px;
background: linear - gradient(45deg, #00ffff, #ff00ff);
            -webkit - background - clip: text;
-webkit - text - fill - color: transparent;
        }
        p { font-size: 1.2rem; margin: 10px 0 15px; max - width: 400px; }
        .button {
            background: linear - gradient(45deg, #00ffff, #ff00ff);
            color: #121212;
            border: none;
padding: 15px 30px;
font - size: 1.2rem;
font - weight: 600;
border - radius: 50px;
cursor: pointer;
transition: transform 0.2s ease, box-shadow 0.2s ease;
box - shadow: 0 0 20px rgba(0, 255, 255, 0.5);
width: 250px;
        }
        .button: hover { transform: scale(1.05); box - shadow: 0 0 30px rgba(255, 0, 255, 0.7); }
        .button: disabled {
background: #555; cursor: not-allowed; box-shadow: none; transform: scale(1); }
        .button.back { background: rgba(255, 255, 255, 0.1); color: white; box - shadow: none; margin - top: 5px; font - size: 1rem; }
# nightmare-btn { background: linear-gradient(45deg, #ff4800, #ff0000); box-shadow: 0 0 20px rgba(255, 0, 0, 0.7); }
# nightmare-btn:hover { box-shadow: 0 0 30px rgba(255, 72, 0, 0.9); }

# game-ui {
position: absolute;
top: 15px;
left: 15px;
right: 15px;
display: flex;
    justify - content: space - between;
    align - items: center;
opacity: 0;
transition: opacity 0.5s;
    font - size: 1.5rem;
    font - weight: 800;
    pointer - events: none;
}
# game-ui > div {
flex: 1 1 0px;
display: flex;
align - items: center;
background: rgba(0, 0, 0, 0.3);
padding: 8px 12px;
border - radius: 12px;
        }
        #score-display { justify-content: flex-start; }
        #level-display { justify-content: center; margin: 0 10px; }
        #coin-display-ingame { justify-content: flex-end; }
        
        #final-score { font-size: 2rem; color: #00ffff; margin-bottom: 20px; font-weight: 600; }
        #coin-display-main { position: absolute; top: 20px; right: 20px; background: rgba(0,0,0,0.5); padding: 10px 20px; border-radius: 20px; font-size: 1.5rem; font-weight: 600; z-index: 20; pointer-events: all; }
        .coin - popup {
position: absolute; color: #ffd700; font-weight: 800; font-size: 1.5rem; animation: floatUp 1s ease-out forwards; pointer-events: none; z-index: 100; }
        @keyframes floatUp { to { transform: translateY(-50px); opacity: 0; } }

        /* --- Shop & Inventory --- */
        .category - tabs {
    display: flex;
    gap: 10px;
        margin - bottom: 10px;
    }
        .category - btn {
    background: #333;
            color: white;
    padding: 10px 20px;
    border: 2px solid #555;
            border - radius: 10px;
    cursor: pointer;
        font - size: 1rem;
    }
        .category - btn.active {
    background: linear - gradient(45deg, #00ffff, #ff00ff);
            color: #121212;
            border - color: #fff;
        }
        .scroll - container {
    width: 80vw; max - width: 600px; overflow - x: auto; overflow - y: hidden; white - space: nowrap; padding: 20px 0; -webkit - overflow - scrolling: touch; scrollbar - color: #555 #222; }
        .item - card {
        display: inline - block; width: 180px; height: 240px; background: #222; border-radius: 15px; border: 2px solid #555; margin: 0 10px; vertical-align: top; text-align: center; padding: 10px; box-sizing: border-box; white-space: normal; display: inline-flex; flex-direction: column; justify-content: space-between; }
        .item - card.preview {
            width: 100 %; height: 80px; background: #333; border-radius: 10px; margin-bottom: 10px; font-size: 2.5rem; line-height: 80px; display: flex; justify-content: center; align-items: center; }
        .item - card h3 { margin: 0 0 5px 0; font - size: 1rem; }
        .item - card p { font - size: 0.9rem; margin: 0 0 10px 0; }
        .item - card.button { width: 100 %; padding: 10px; font - size: 1rem; border - radius: 10px; }
        .button.equip {
                background: linear - gradient(45deg, #0f0, #0af); }
        .button.unequip {
                    background: linear - gradient(45deg, #f00, #fa0); }

        /* --- Settings Toggle Switch --- */
        .setting - row { display: flex; justify - content: space - between; align - items: center; width: 100 %; padding: 10px 0; font - size: 1.2rem; }
        .toggle -switch { position: relative; display: inline - block; width: 60px; height: 34px; }
        .toggle -switch input { opacity: 0; width: 0; height: 0;
                    }
        .slider {
                    position: absolute; cursor: pointer; top: 0; left: 0; right: 0; bottom: 0; background - color: #333; transition: .4s; border-radius: 34px; }
        .slider: before { position: absolute; content: ""; height: 26px; width: 26px; left: 4px; bottom: 4px; background - color: white; transition: .4s; border - radius: 50 %; }
                    input: checked + .slider {
                        background: linear - gradient(45deg, #00ffff, #ff00ff); }
        input: checked + .slider:before { transform: translateX(26px); }
        
        /* --- Style Modes --- */
        .retro - mode, .retro - mode body { font - family: 'Press Start 2P', cursive!important; }
        .retro - mode h1, .retro - mode p, .retro - mode.button, .retro - mode.setting - row, .retro - mode #game-ui, .retro-mode #final-score, .retro-mode #coin-display-main, .retro-mode .item-card, .retro-mode .category-btn { font-family: 'Press Start 2P', cursive; text-shadow: 2px 2px #000; }
        .retro - mode.screen, .retro - mode.button, .retro - mode.slider, .retro - mode canvas, .retro - mode.item - card, .retro - mode.item - card.preview, .retro - mode #game-ui > div, .retro-mode .category-btn { border-radius: 0 !important; }
        .retro - mode.slider:before { border - radius: 0!important; }
        .retro - mode.button {
                            background: #00ffff !important; color: #000 !important; box-shadow: none !important; border: 3px solid #fff; text-transform: uppercase; font-size: 0.8rem; }
        .retro - mode.button:hover {
                                background: #ff00ff !important; color: #000 !important; transform: none !important; }
        .retro - mode.button:disabled {
                                    background: #555 !important; border-color: #777; color: #999 !important; }
        .retro - mode h1 {
                                        background: none!important; -webkit - background - clip: unset!important; -webkit - text - fill - color: #00ffff !important; font-size: 2.5rem; }
        .retro - mode #nightmare-btn { background: #ff0000 !important; border-color: #ff4800; }
        .retro - mode.button.back {
                                            background: #333 !important; color: #fff !important; }
        .retro - mode #game-ui { font-size: 1.2rem; }
        .retro - mode #final-score {font-size: 1.5rem; }
        .retro - mode #coin-display-main { font-size: 1rem; border-radius: 0; }

        .cartoon - mode, .cartoon - mode body { font - family: 'Luckiest Guy', cursive!important; }
        .cartoon - mode h1, .cartoon - mode p, .cartoon - mode.button, .cartoon - mode.setting - row, .cartoon - mode #game-ui, .cartoon-mode #final-score, .cartoon-mode #coin-display-main, .cartoon-mode .item-card, .cartoon-mode .category-btn { font-family: 'Luckiest Guy', cursive; text-shadow: 2px 2px 0 #000, -2px -2px 0 #000, 2px -2px 0 #000, -2px 2px 0 #000; letter-spacing: 1px; }
        .cartoon - mode.screen, .cartoon - mode.button, .cartoon - mode canvas, .cartoon - mode.item - card, .cartoon - mode.item - card.preview, .cartoon - mode #game-ui > div, .cartoon-mode .category-btn { border: 4px solid #000; box-shadow: 8px 8px 0px rgba(0,0,0,0.5); border-radius: 12px; }
        .cartoon - mode.button { border - radius: 25px; }
        .cartoon - mode.button:hover { transform: translateY(-2px) translateX(-2px); box - shadow: 10px 10px 0px rgba(0,0,0,0.5); }
        .cartoon - mode.button:disabled { box - shadow: 8px 8px 0px rgba(0,0,0,0.5); transform: none; }
        .cartoon - mode h1 {
                                                    text - shadow: 4px 4px 0 #000; }

        /* --- Language & RTL Styles --- */
        [lang = "he"] body, [lang = "he"].screen { font - family: 'Rubik', sans - serif; }
                                                    [dir= "rtl"] { text - align: right; }
                                                    [dir = "rtl"] #coin-display-main { right: auto; left: 20px; }
        [dir = "rtl"] #score-display { justify-content: flex-end; }
        [dir = "rtl"] #coin-display-ingame { justify-content: flex-start; }
    </ style >
                                            </ head >
                                            < body >
                                            
                                                < div id = "coin-display-main" >💰 0 </ div >
                                            
                                                < canvas id = "gameCanvas" ></ canvas >
                                            

                                                < div class= "ui-container" >
                                            
                                                    < !--Intro Screen-- >
                                            
                                                    < div id = "intro-screen" class= "screen" >
                                            
                                                        < h1 data - key = "title" > Directional Defender </ h1 >
                                            
                                                        < p data - key = "intro_p" > Select a game mode. Destroy incoming enemies before they reach the core!</p>
            <button id="play-button" class= "button" data - key = "play" > Play </ button >
            < button id = "shop-button" class= "button" data - key = "shop" > Shop </ button >
            < button id = "inventory-button" class= "button" data - key = "inventory" > Inventory </ button >
            < button id = "settings-button" class= "button" data - key = "settings" > Settings </ button >
        </ div >
        < div id = "mode-screen" class= "screen hidden" >
            < h1 data - key = "select_mode" > Select Mode </ h1 >
            < button id = "levels-mode-button" class= "button" data - key = "levels" > Levels </ button >
            < button id = "difficulty-mode-button" class= "button" data - key = "difficulties" > Difficulties </ button >
            < button id = "back-to-intro-button" class= "button back" data - key = "main_menu" > Main Menu </ button >
        </ div >
        < div id = "difficulty-screen" class= "screen hidden" >
            < h1 data - key = "select_difficulty" > Select Difficulty </ h1 >
            < button class= "button difficulty-btn" data - difficulty = "easy" data - key = "easy" > Easy </ button >
            < button class= "button difficulty-btn" data - difficulty = "normal" data - key = "normal" > Normal </ button >
            < button class= "button difficulty-btn" data - difficulty = "hard" data - key = "hard" > Hard </ button >
            < button class= "button difficulty-btn" data - difficulty = "impossible" data - key = "impossible" > Impossible </ button >
            < button class= "button difficulty-btn" id = "nightmare-btn" data - difficulty = "nightmare" data - key = "nightmare" > Nightmare </ button >
            < button id = "back-to-mode-button" class= "button back" data - key = "back_to_modes" > Back to Modes</button>
        </div>
        <div id = "shop-screen" class= "screen hidden" >
            < h1 data - key = "shop" > Shop </ h1 >
            < div id = "shop-category-tabs" class= "category-tabs" ></ div >
            < div id = "shop-container" class= "scroll-container" ></ div >
            < button id = "back-to-intro-from-shop-button" class= "button back" data - key = "main_menu" > Main Menu </ button >
        </ div >
        < div id = "inventory-screen" class= "screen hidden" >
            < h1 data - key = "inventory" > Inventory </ h1 >
            < div id = "inventory-category-tabs" class= "category-tabs" ></ div >
            < div id = "inventory-container" class= "scroll-container" ></ div >
            < button id = "back-to-intro-from-inventory-button" class= "button back" data - key = "main_menu" > Main Menu </ button >
        </ div >
        < div id = "settings-screen" class= "screen hidden" >
            < h1 data - key = "settings" > Settings </ h1 >
            < div class= "setting-row" >
                < span data - key = "music" > Music </ span >
                < label class= "toggle-switch" >< input type = "checkbox" id = "music-toggle" checked>< span class= "slider" ></ span ></ label >
            </ div >
            < div class= "setting-row" >
                < span data - key = "sfx" > Sound Effects </ span >
                < label class= "toggle-switch" >< input type = "checkbox" id = "sfx-toggle" checked>< span class= "slider" ></ span ></ label >
            </ div >
             < div class= "setting-row" >
                < span data - key = "language" > Language </ span >
                < button id = "language-button" class= "button" style = "width: 150px; font-size: 1rem; padding: 10px;" > English </ button >
            </ div >
            < button id = "back-to-intro-from-settings-button" class= "button back" data - key = "main_menu" > Main Menu </ button >
        </ div >
        < div id = "game-ui" >
            < div id = "score-display" ></ div >
            < div id = "level-display" ></ div >
            < div id = "coin-display-ingame" ></ div >
        </ div >
        < div id = "game-over-screen" class= "screen hidden" >
            < h1 data - key = "game_over" > Game Over </ h1 >
            < p id = "final-score" ></ p >
            < button id = "restart-button" class= "button" data - key = "play_again" > Play Again </ button >
            < button id = "main-menu-button" class= "button back" data - key = "main_menu" > Main Menu </ button >
        </ div >
    </ div >

    < script >
        // --- Setup ---
        const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');
const screens = {
            intro: document.getElementById('intro-screen'), mode: document.getElementById('mode-screen'),
            difficulty: document.getElementById('difficulty-screen'), settings: document.getElementById('settings-screen'),
            shop: document.getElementById('shop-screen'), inventory: document.getElementById('inventory-screen'),
            gameOver: document.getElementById('game-over-screen'),
        };
const gameUI = document.getElementById('game-ui');

// --- Translations ---
const translations = {
            en: {
title: "Directional Defender", intro_p: "Select a game mode. Destroy incoming enemies before they reach the core!",
                play: "Play", shop: "Shop", inventory: "Inventory", settings: "Settings",
                select_mode: "Select Mode", levels: "Levels", difficulties: "Difficulties", main_menu: "Main Menu",
                select_difficulty: "Select Difficulty", easy: "Easy", normal: "Normal", hard: "Hard", impossible: "Impossible", nightmare: "Nightmare",
                back_to_modes: "Back to Modes", 
                retro_style_name: "Retro Style", retro_style_desc: "8-bit graphics and chiptune audio!",
                cartoon_style_name: "Cartoon Style", cartoon_style_desc: "Wacky, hand-drawn visuals and sounds!",
                mirrored_shooting_name: "Mirrored Shooting", mirrored_shooting_desc: "Shoots a projectile behind you as well.",
                multi_fingers_name: "Multi Fingers", multi_fingers_desc: "Hitting an enemy shoots 3 random projectiles.",
                owned: "Owned", buy: "Buy", equip: "Equip", unequip: "Unequip",
                music: "Music", sfx: "Sound Effects", language: "Language",
                score: "Score", level: "Level", game_over: "Game Over", your_score: "Your Score", play_again: "Play Again",
                inventory_empty: "You don't own any items. Visit the shop!", lang_name: "English",
                category_style: "Game Styles", category_ability: "Abilities"
            },
            he:
{
title: "מגן כיווני", intro_p: "בחרו מצב משחק. השמידו את האויבים לפני שהם מגיעים לליבה!",
                play: "שחק", shop: "חנות", inventory: "ציוד", settings: "הגדרות",
                select_mode: "בחירת מצב", levels: "שלבים", difficulties: "רמות קושי", main_menu: "תפריט ראשי",
                select_difficulty: "בחירת רמת קושי", easy: "קל", normal: "רגיל", hard: "קשה", impossible: "בלתי אפשרי", nightmare: "סיוט",
                back_to_modes: "חזרה למצבים",
                retro_style_name: "סגנון רטרו", retro_style_desc: "גרפיקת 8-ביט וצלילי צ'יפטיון!",
                cartoon_style_name: "סגנון מצויר", cartoon_style_desc: "גרפיקה מצוירת וצלילים מצחיקים!",
                mirrored_shooting_name: "ירי מראה", mirrored_shooting_desc: "יורה קליע גם מאחוריך.",
                multi_fingers_name: "ריבוי אצבעות", multi_fingers_desc: "פגיעה באויב יורה 3 קליעים אקראיים.",
                owned: "בבעלותך", buy: "קנה", equip: "צייד", unequip: "הסר ציוד",
                music: "מוזיקה", sfx: "אפקטים", language: "שפה",
                score: "ניקוד", level: "שלב", game_over: "המשחק נגמר", your_score: "הניקוד שלך", play_again: "שחק שוב",
                inventory_empty: "אין ברשותך פריטים. בקר בחנות!", lang_name: "עברית",
                category_style: "סגנונות משחק", category_ability: "יכולות"
            }
        };

// --- Player Data ---
let playerData = { };
const SHOP_ITEMS = [
            { id: 'retro_style', price: 100, preview: '👾', category: 'style' },
            { id: 'cartoon_style', price: 250, preview: '🎨', category: 'style' },
            { id: 'mirrored_shooting', price: 300, preview: '↔️', category: 'ability' },
            { id: 'multi_fingers', price: 750, preview: '💥', category: 'ability' }
        ];

function loadData()
{
    const savedData = JSON.parse(localStorage.getItem('directionalDefenderData'));
    playerData = { coins: 0, ownedItems: [], equippedItem: null, equippedAbility: null, language: 'en', ...savedData }
    ;
    updateCoinDisplay();
    setLanguage(playerData.language, true);
}

function saveData()
{
    localStorage.setItem('directionalDefenderData', JSON.stringify(playerData));
    updateCoinDisplay();
}

function setLanguage(lang, isInitialLoad = false)
{
    playerData.language = lang;
    document.documentElement.lang = lang;
    document.documentElement.dir = lang === 'he' ? 'rtl' : 'ltr';
    document.querySelectorAll('[data-key]').forEach(elem => {
        const key = elem.dataset.key;
        if (translations[lang][key]) elem.textContent = translations[lang][key];
    });
    document.getElementById('language-button').textContent = translations[lang].lang_name;
    if (!isInitialLoad) applyEquippedStyle();
}

let audioContext, musicEnabled = true, sfxEnabled = true, musicInterval;
let shopCategory = 'style';
let inventoryCategory = 'style';

function initAudio()
{
    if (audioContext) return;
    audioContext = new(window.AudioContext || window.webkitAudioContext)();
    if (musicEnabled) startMusic();
}

function playSound(type)
{
    if (!audioContext || !sfxEnabled) return;
    const isRetro = playerData.equippedItem === 'retro_style';
    const isCartoon = playerData.equippedItem === 'cartoon_style';
    const now = audioContext.currentTime;
    const osc = audioContext.createOscillator(); const gain = audioContext.createGain();
    osc.connect(gain); gain.connect(audioContext.destination);

    if (typeof type === 'object' && type.type === 'note')
    {
        osc.type = isRetro ? 'square' : 'sine';
        osc.frequency.setValueAtTime(type.freq, type.time);
        gain.gain.setValueAtTime(isRetro ? 0.2 : 0.1, type.time);
        gain.gain.exponentialRampToValueAtTime(0.0001, type.time + type.dur);
        osc.start(type.time); osc.stop(type.time + type.dur);
        return;
    }

    switch (type)
    {
        case 'shoot':
            osc.type = isRetro ? 'square' : (isCartoon ? 'triangle' : 'triangle');
            osc.frequency.setValueAtTime(isRetro ? 1320 : (isCartoon ? 1200 : 880), now);
            if (isCartoon) osc.frequency.exponentialRampToValueAtTime(400, now + 0.1);
            gain.gain.setValueAtTime(isRetro ? 0.05 : 0.1, now);
            gain.gain.exponentialRampToValueAtTime(0.0001, now + 0.1);
            osc.start(now); osc.stop(now + 0.1);
            break;
        case 'hit':
            if (isCartoon)
            {
                const noise = audioContext.createBufferSource();
                const buffer = audioContext.createBuffer(1, audioContext.sampleRate * 0.2, audioContext.sampleRate);
                const data = buffer.getChannelData(0);
                for (let i = 0; i < data.length; i++) { data[i] = Math.random() * 2 - 1; }
                noise.buffer = buffer;
                const filter = audioContext.createBiquadFilter();
                filter.type = 'lowpass';
                filter.frequency.setValueAtTime(2000, now);
                filter.frequency.linearRampToValueAtTime(100, now + 0.1);
                noise.connect(filter).connect(gain);
                gain.gain.setValueAtTime(0.3, now);
                gain.gain.exponentialRampToValueAtTime(0.0001, now + 0.2);
                noise.start(now); noise.stop(now + 0.2);
            }
            else
            {
                osc.type = isRetro ? 'square' : 'sine';
                osc.frequency.setValueAtTime(isRetro ? 110 : 220, now);
                gain.gain.setValueAtTime(isRetro ? 0.3 : 0.2, now);
                gain.gain.exponentialRampToValueAtTime(0.0001, now + 0.2);
                osc.start(now); osc.stop(now + 0.2);
            }
            break;
        case 'damage':
            osc.type = isRetro ? 'square' : (isCartoon ? 'sine' : 'sawtooth');
            osc.frequency.setValueAtTime(isRetro ? 87 : (isCartoon ? 150 : 110), now);
            if (isCartoon) osc.frequency.exponentialRampToValueAtTime(100, now + 0.1);
            gain.gain.setValueAtTime(isCartoon ? 0.4 : 0.3, now);
            gain.gain.exponentialRampToValueAtTime(0.0001, now + 0.2);
            osc.start(now); osc.stop(now + 0.2);
            break;
        case 'levelUp':
            playSound({ type: 'note', freq: 440, dur: 0.1, time: now });
            playSound({ type: 'note', freq: 554, dur: 0.1, time: now + 0.1 });
            playSound({ type: 'note', freq: 659, dur: 0.1, time: now + 0.2 });
            break;
        case 'gameOver':
            if (isCartoon)
            {
                osc.type = 'triangle';
                osc.frequency.setValueAtTime(200, now);
                osc.frequency.exponentialRampToValueAtTime(50, now + 0.8);
                gain.gain.setValueAtTime(0.3, now);
                gain.gain.exponentialRampToValueAtTime(0.0001, now + 0.8);
                osc.start(now); osc.stop(now + 0.8);
            }
            else
            {
                osc.type = 'square'; osc.frequency.setValueAtTime(130, now);
                gain.gain.setValueAtTime(0.2, now); gain.gain.exponentialRampToValueAtTime(0.0001, now + 1);
                osc.start(now); osc.stop(now + 1);
            }
            break;
    }
}

function startMusic()
{
    stopMusic(); if (!audioContext) return;
    const isRetro = playerData.equippedItem === 'retro_style';
    const isCartoon = playerData.equippedItem === 'cartoon_style';
    const retroNotes = [130.81, 130.81, 261.63, 196.00, 164.81, 164.81, 130.81, 196.00];
    const cartoonNotes = [261.63, null, 293.66, 329.63, null, 261.63, 329.63, 349.23, 392.00, null];
    const modernNotes = [130.81, 164.81, 196.00, 261.63, 196.00, 164.81, 130.81, 98.00, 116.54, 146.83, 174.61, 233.08, 174.61, 146.83, 116.54, 87.31];
    const notes = isRetro ? retroNotes : (isCartoon ? cartoonNotes : modernNotes);
    const tempo = isRetro ? 150 : (isCartoon ? 180 : 250);
    const noteDuration = isRetro ? 0.15 : (isCartoon ? 0.15 : 0.2);
    const oscType = isRetro ? 'square' : (isCartoon ? 'sine' : 'triangle');
    let noteIndex = 0;
    musicInterval = setInterval(() => {
        if (musicEnabled && audioContext && notes[noteIndex])
        {
            const osc = audioContext.createOscillator(); const gain = audioContext.createGain();
            osc.connect(gain); gain.connect(audioContext.destination);
            osc.type = oscType; osc.frequency.value = notes[noteIndex];
            gain.gain.setValueAtTime(0.08, audioContext.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.0001, audioContext.currentTime + noteDuration);
            osc.start(); osc.stop(audioContext.currentTime + noteDuration);
        }
        noteIndex = (noteIndex + 1) % notes.length;
    }, tempo);
}

function stopMusic() { if (musicInterval) { clearInterval(musicInterval); musicInterval = null; } }

const musicToggle = document.getElementById('music-toggle');
const sfxToggle = document.getElementById('sfx-toggle');
const languageButton = document.getElementById('language-button');
musicToggle.addEventListener('change', (e) => { musicEnabled = e.target.checked; if (musicEnabled) startMusic(); else stopMusic(); });
sfxToggle.addEventListener('change', (e) => { sfxEnabled = e.target.checked; });
languageButton.addEventListener('click', () => { setLanguage(playerData.language === 'en' ? 'he' : 'en'); saveData(); });

function applyEquippedStyle()
{
    const isRetro = playerData.equippedItem === 'retro_style';
    const isCartoon = playerData.equippedItem === 'cartoon_style';
    document.body.classList.toggle('retro-mode', isRetro);
    document.body.classList.toggle('cartoon-mode', isCartoon);
    applyCanvasAntiAliasing();
    if (musicEnabled && audioContext) startMusic();
}

function renderCategoryTabs(containerId, activeCategory, onTabClick)
{
    const container = document.getElementById(containerId);
    container.innerHTML = '';
    const categories = ['style', 'ability'];
    categories.forEach(cat => {
    const btn = document.createElement('button');
    btn.className = `category - btn ${ activeCategory === cat ? 'active' : ''}`;
    btn.dataset.category = cat;
    btn.dataset.key = `category_${ cat}`;
    btn.textContent = translations[playerData.language][`category_${ cat}`];
    btn.addEventListener('click', () => onTabClick(cat));
    container.appendChild(btn);
});
        }

        function renderShop()
{
    renderCategoryTabs('shop-category-tabs', shopCategory, (cat) => {
        shopCategory = cat;
        renderShop();
    });
    const container = document.getElementById('shop-container'); container.innerHTML = '';
    SHOP_ITEMS.filter(item => item.category === shopCategory).forEach(item => {
    const lang = playerData.language;
    const isOwned = playerData.ownedItems.includes(item.id);
    const canAfford = playerData.coins >= item.price;
    const card = document.createElement('div'); card.className = 'item-card';
    card.innerHTML = `
                    < div class= "preview" >${ item.preview}</ div >
                    < h3 data - key = "${item.id}_name" >${ translations[lang][`${ item.id} _name`]}</ h3 >
                    < p data - key = "${item.id}_desc" >${ translations[lang][`${ item.id} _desc`]}</ p >
                    < p >💰 ${ item.price}</ p >
                    < button class= "button buy-btn" data - item - id = "${item.id}" ${ isOwned || !canAfford ? 'disabled' : ''}
data - key = "${isOwned ? 'owned' : 'buy'}" >
                        ${ translations[lang][isOwned ? 'owned' : 'buy']}
                    </ button >`;
container.appendChild(card);
            });
document.querySelectorAll('.buy-btn:not([disabled])').forEach(btn => { btn.addEventListener('click', () => buyItem(btn.dataset.itemId)); });
        }

        function buyItem(itemId)
{
    const item = SHOP_ITEMS.find(i => i.id === itemId);
    if (item && playerData.coins >= item.price && !playerData.ownedItems.includes(itemId))
    {
        playerData.coins -= item.price; playerData.ownedItems.push(itemId);
        saveData(); renderShop();
    }
}

function renderInventory()
{
    const ownedItems = playerData.ownedItems.map(id => SHOP_ITEMS.find(i => i.id === id));
    const ownedCategories = [...new Set(ownedItems.map(i => i && i.category))].filter(Boolean);

    const tabsContainer = document.getElementById('inventory-category-tabs');
    if (ownedCategories.length > 1)
    {
        tabsContainer.style.display = 'flex';
        renderCategoryTabs('inventory-category-tabs', inventoryCategory, (cat) => {
            inventoryCategory = cat;
            renderInventory();
        });
    }
    else
    {
        tabsContainer.style.display = 'none';
    }

    const container = document.getElementById('inventory-container'); container.innerHTML = '';
    const lang = playerData.language;
    const itemsToShow = ownedItems.filter(item => item && item.category === inventoryCategory);

    if (itemsToShow.length === 0 && playerData.ownedItems.length > 0)
    {
        inventoryCategory = ownedCategories[0];
        renderInventory();
        return;
    }

    if (playerData.ownedItems.length === 0)
    {
        container.innerHTML = `< p data - key = "inventory_empty" >${ translations[lang].inventory_empty}</ p >`; return;
    }

    itemsToShow.forEach(item => {
    if (!item) return;
    const isEquipped = playerData.equippedItem === item.id || playerData.equippedAbility === item.id;
    const card = document.createElement('div'); card.className = 'item-card';
    card.innerHTML = `
                    < div class= "preview" >${ item.preview}</ div >
                    < h3 data - key = "${item.id}_name" >${ translations[lang][`${ item.id} _name`]}</ h3 >
                    < p data - key = "${item.id}_desc" >${ translations[lang][`${ item.id} _desc`]}</ p >
                    < button class= "button ${isEquipped ? 'unequip' : 'equip'} equip-btn" data - item - id = "${item.id}" data - key = "${isEquipped ? 'unequip' : 'equip'}" >
                        ${ translations[lang][isEquipped ? 'unequip' : 'equip']}
                    </ button >`;
container.appendChild(card);
            });
document.querySelectorAll('.equip-btn').forEach(btn => { btn.addEventListener('click', () => toggleEquipItem(btn.dataset.itemId)); });
        }
        
        function toggleEquipItem(itemId)
{
    const item = SHOP_ITEMS.find(i => i.id === itemId);
    if (!item) return;

    if (item.category === 'style')
    {
        playerData.equippedItem = (playerData.equippedItem === itemId) ? null : itemId;
    }
    else if (item.category === 'ability')
    {
        playerData.equippedAbility = (playerData.equippedAbility === itemId) ? null : itemId;
    }
    saveData(); renderInventory(); applyEquippedStyle();
}

const coinDisplay = document.getElementById('coin-display-main');
function updateCoinDisplay()
{
    const text = `💰 ${ playerData.coins}`;
    coinDisplay.textContent = text;
    document.getElementById('coin-display-ingame').textContent = text;
}

function addButtonClickListener(id, callback) { document.getElementById(id).addEventListener('click', callback); }
addButtonClickListener('play-button', () => { initAudio(); showScreen('mode'); });
addButtonClickListener('settings-button', () => showScreen('settings'));
addButtonClickListener('shop-button', () => { shopCategory = 'style'; renderShop(); showScreen('shop'); });
addButtonClickListener('inventory-button', () => { inventoryCategory = 'style'; renderInventory(); showScreen('inventory'); });
addButtonClickListener('levels-mode-button', () => startLevelsMode());
addButtonClickListener('difficulty-mode-button', () => showScreen('difficulty'));
addButtonClickListener('back-to-intro-button', () => showScreen('intro'));
addButtonClickListener('back-to-mode-button', () => showScreen('mode'));
addButtonClickListener('back-to-intro-from-shop-button', () => showScreen('intro'));
addButtonClickListener('back-to-intro-from-inventory-button', () => showScreen('intro'));
addButtonClickListener('back-to-intro-from-settings-button', () => showScreen('intro'));
addButtonClickListener('restart-button', () => {
    if (gameMode === 'levels') startLevelsMode();
    else if (gameMode === 'difficulty') startDifficultyMode(lastDifficulty);
});
addButtonClickListener('main-menu-button', () => { showScreen('intro'); canvas.style.backgroundColor = '#1a1a1a'; });
document.querySelectorAll('.difficulty-btn').forEach(btn => { btn.addEventListener('click', () => startDifficultyMode(btn.dataset.difficulty)); });

let player, projectiles, enemies, particles, score, animationId, gameMode, lastDifficulty, spawnTimerId, currentLevel, levelUpScore, currentDifficulty;
const difficulties = {
            easy:       { speed: 0.8, minR: 15, maxR: 25, spawn: 1200, coinChance: 0.1, coinAmount: 3 },
            normal: { speed: 1.2, minR: 10, maxR: 20, spawn: 1000, coinChance: 0.15, coinAmount: 5 },
            hard: { speed: 1.6, minR: 8,  maxR: 15, spawn: 700, coinChance: 0.2, coinAmount: 8 },
            impossible: { speed: 2.2, minR: 5,  maxR: 12, spawn: 450, coinChance: 0.25, coinAmount: 12 },
            nightmare: { speed: 2.8, minR: 4,  maxR: 10, spawn: 300, bgColor: '#3d0000', trailColor: 'rgba(100, 0, 0, 0.25)', coinChance: 0.3, coinAmount: 20 }
        };
function awardCoins(amount, x, y)
{
    playerData.coins += amount; saveData();
    const rect = canvas.getBoundingClientRect(); const popup = document.createElement('div');
    popup.className = 'coin-popup'; popup.textContent = `+${ amount} 💰`;
    popup.style.left = `${ rect.left + x}
    px`; popup.style.top = `${ rect.top + y}
    px`;
    document.body.appendChild(popup); setTimeout(() => popup.remove(), 1000);
}

class Player
{
    constructor(x, y, radius, color) { this.x = x; this.y = y; this.radius = radius; this.color = color; this.maxHealth = 10; this.health = this.maxHealth; }
    draw()
    {
        const isRetro = playerData.equippedItem === 'retro_style';
        const isCartoon = playerData.equippedItem === 'cartoon_style';
        if (isRetro)
        {
            ctx.fillStyle = this.color; ctx.fillRect(this.x - this.radius, this.y - this.radius, this.radius * 2, this.radius * 2);
            const blockWidth = (this.radius * 2) / this.maxHealth;
            for (let i = 0; i < this.maxHealth; i++)
            {
                ctx.fillStyle = i < this.health ? '#ff00ff' : '#555';
                ctx.fillRect(this.x - this.radius + (i * blockWidth), this.y - this.radius - 12, blockWidth - 2, 8);
            }
        }
        else if (isCartoon)
        {
            const wobble = Math.sin(Date.now() / 150) * this.radius * 0.05;
            ctx.fillStyle = 'black'; ctx.beginPath(); ctx.arc(this.x, this.y + wobble, this.radius + 4, 0, Math.PI * 2); ctx.fill();
            ctx.fillStyle = this.color; ctx.beginPath(); ctx.arc(this.x, this.y + wobble, this.radius, 0, Math.PI * 2); ctx.fill();
            ctx.fillStyle = '#222'; ctx.beginPath(); ctx.arc(this.x, this.y + wobble, this.radius * 0.7, Math.PI * 0.1, Math.PI * 0.9); ctx.fill();
            const heartSize = this.radius * 0.4; const totalHealthWidth = this.maxHealth * (heartSize + 4);
            const startX = this.x - totalHealthWidth / 2; const heartY = this.y - this.radius - 20;
            ctx.save(); ctx.font = `${ heartSize * 2}
            px 'Luckiest Guy'`; ctx.textBaseline = 'middle';
            ctx.fillStyle = 'black'; for (let i = 0; i < this.maxHealth; i++) { ctx.fillText('♥', startX + i * (heartSize + 4) + 2, heartY + 2); }
            ctx.fillStyle = '#555'; for (let i = 0; i < this.maxHealth; i++) { ctx.fillText('♥', startX + i * (heartSize + 4), heartY); }
            ctx.fillStyle = '#ff4d4d'; for (let i = 0; i < this.health; i++) { ctx.fillText('♥', startX + i * (heartSize + 4), heartY); }
            ctx.restore();
        }
        else
        {
            ctx.save(); ctx.beginPath(); ctx.arc(this.x, this.y, this.radius + 10, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(0, 255, 255, ${ 0.1 * (this.health / this.maxHealth)})`; ctx.fill(); ctx.restore();
            ctx.beginPath(); ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2); ctx.fillStyle = this.color; ctx.fill();
            ctx.save(); ctx.lineWidth = 5; ctx.strokeStyle = '#ff00ff'; ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 5, -Math.PI / 2, -Math.PI / 2 + (Math.PI * 2 * (this.health / this.maxHealth)));
            ctx.stroke(); ctx.strokeStyle = '#00ffff'; ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 5, -Math.PI / 2 + (Math.PI * 2 * (this.health / this.maxHealth)), -Math.PI / 2 + Math.PI * 2);
            ctx.stroke(); ctx.restore();
        }
    }
}
class Projectile
{
    constructor(x, y, radius, color, velocity) { this.x = x; this.y = y; this.radius = radius; this.color = color; this.velocity = velocity; this.alpha = 1; }
    draw()
    {
        const isRetro = playerData.equippedItem === 'retro_style';
        const isCartoon = playerData.equippedItem === 'cartoon_style';
        ctx.save(); ctx.globalAlpha = this.alpha; ctx.fillStyle = this.color;
        if (isRetro) { ctx.fillRect(this.x - this.radius, this.y - this.radius, this.radius * 2, this.radius * 2); }
        else if (isCartoon)
        {
            ctx.lineWidth = 3; ctx.strokeStyle = 'black'; ctx.beginPath();
            ctx.moveTo(this.x - this.radius, this.y + this.radius); ctx.lineTo(this.x, this.y - this.radius * 1.5);
            ctx.lineTo(this.x + this.radius, this.y + this.radius);
            ctx.arcTo(this.x, this.y + this.radius * 1.5, this.x - this.radius, this.y + this.radius, this.radius);
            ctx.closePath(); ctx.fill(); ctx.stroke();
        }
        else { ctx.beginPath(); ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2); ctx.fill(); }
        ctx.restore();
    }
    update() { this.x += this.velocity.x; this.y += this.velocity.y; this.alpha -= 0.02; this.draw(); }
}
class Enemy
{
    constructor(x, y, radius, color, velocity) { this.x = x; this.y = y; this.radius = radius; this.color = color; this.velocity = velocity; }
    draw()
    {
        const isRetro = playerData.equippedItem === 'retro_style';
        const isCartoon = playerData.equippedItem === 'cartoon_style';
        if (isRetro)
        {
            ctx.fillStyle = this.color; ctx.fillRect(this.x - this.radius, this.y - this.radius, this.radius * 2, this.radius * 2);
        }
        else if (isCartoon)
        {
            const wobble = Math.sin(Date.now() / 120 + this.x) * this.radius * 0.05;
            const x = this.x; const y = this.y + wobble;
            ctx.fillStyle = 'black'; ctx.beginPath(); ctx.arc(x, y, this.radius + 3, 0, Math.PI * 2); ctx.fill();
            ctx.fillStyle = this.color; ctx.beginPath(); ctx.arc(x, y, this.radius, 0, Math.PI * 2); ctx.fill();
            const eyeRadius = this.radius * 0.15; const eyeOffsetX = this.radius * 0.4;
            ctx.fillStyle = 'white'; ctx.beginPath(); ctx.arc(x - eyeOffsetX, y - eyeOffsetX, eyeRadius, 0, Math.PI * 2); ctx.fill();
            ctx.beginPath(); ctx.arc(x + eyeOffsetX, y - eyeOffsetX, eyeRadius, 0, Math.PI * 2); ctx.fill();
            ctx.fillStyle = 'black'; ctx.beginPath(); ctx.arc(x - eyeOffsetX, y - eyeOffsetX, eyeRadius * 0.5, 0, Math.PI * 2); ctx.fill();
            ctx.beginPath(); ctx.arc(x + eyeOffsetX, y - eyeOffsetX, eyeRadius * 0.5, 0, Math.PI * 2); ctx.fill();
        }
        else
        {
            ctx.fillStyle = this.color; ctx.beginPath(); ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2); ctx.fill();
        }
    }
    update() { this.x += this.velocity.x; this.y += this.velocity.y; this.draw(); }
}
class Particle
{
    constructor(x, y, radius, color, velocity, type = 'default') { this.x = x; this.y = y; this.radius = radius; this.color = color; this.velocity = velocity; this.alpha = 1; this.type = type; }
    draw()
    {
        const isRetro = playerData.equippedItem === 'retro_style';
        const isCartoon = playerData.equippedItem === 'cartoon_style';
        ctx.save(); ctx.globalAlpha = this.alpha; ctx.fillStyle = this.color;
        if (this.type === 'shoot_effect')
        {
            ctx.font = `${ this.radius}
            px 'Luckiest Guy'`;
            ctx.fillStyle = `rgba(255, 220, 0, ${ this.alpha})`; ctx.strokeStyle = 'black'; ctx.lineWidth = 2;
            ctx.strokeText('POW!', this.x, this.y); ctx.fillText('POW!', this.x, this.y);
        }
        else if (isRetro) { ctx.fillRect(this.x - this.radius, this.y - this.radius, this.radius * 2, this.radius * 2); }
        else if (isCartoon)
        {
            ctx.lineWidth = 4; ctx.strokeStyle = this.color; ctx.beginPath();
            for (let i = 0; i < 5; i++)
            {
                let angle = i * (Math.PI * 2 / 5); ctx.lineTo(this.x + Math.cos(angle) * this.radius * this.alpha, this.y + Math.sin(angle) * this.radius * this.alpha);
                angle += Math.PI / 5; ctx.lineTo(this.x + Math.cos(angle) * this.radius * 0.5 * this.alpha, this.y + Math.sin(angle) * this.radius * 0.5 * this.alpha);
            }
            ctx.closePath(); ctx.stroke();
        }
        else { ctx.beginPath(); ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2); ctx.fill(); }
        ctx.restore();
    }
    update()
    {
        if (this.type === 'shoot_effect') { this.radius += 1; }
        else { this.velocity.x *= 0.98; this.velocity.y *= 0.98; this.x += this.velocity.x; this.y += this.velocity.y; }
        this.alpha -= 0.05; this.draw();
    }
}

function init()
{
    const centerX = canvas.width / 2; const centerY = canvas.height / 2;
    player = new Player(centerX, centerY, canvas.width * 0.04, 'white');
    projectiles = []; enemies = []; particles = []; score = 0;
    updateScoreUI();
    updateLevelUI();
}
function spawnEnemies()
{
    let spawnInterval, speed, minR, maxR;
    if (gameMode === 'levels')
    {
        spawnInterval = Math.max(200, 1200 - (currentLevel * 70)); speed = 1 + (currentLevel * 0.15); minR = Math.max(5, 15 - currentLevel); maxR = Math.max(8, 25 - currentLevel);
    }
    else
    {
        spawnInterval = currentDifficulty.spawn; speed = currentDifficulty.speed; minR = currentDifficulty.minR; maxR = currentDifficulty.maxR;
    }
    spawnTimerId = setTimeout(spawnEnemies, spawnInterval);
    const radius = Math.random() * (maxR - minR) + minR;
    let x, y;
    if (Math.random() < 0.5) { x = Math.random() < 0.5 ? 0 - radius : canvas.width + radius; y = Math.random() * canvas.height; } else { x = Math.random() * canvas.width; y = Math.random() < 0.5 ? 0 - radius : canvas.height + radius; }
    const angle = Math.atan2(player.y - y, player.x - x);
    const velocity = { x: Math.cos(angle) * speed, y: Math.sin(angle) * speed };
const color = `hsl(${Math.random() * 360}, 70 %, 60 %)`;
enemies.push(new Enemy(x, y, radius, color, velocity));
        }
        function animate()
{
    animationId = requestAnimationFrame(animate);
    ctx.fillStyle = (gameMode === 'difficulty' && lastDifficulty === 'nightmare') ? currentDifficulty.trailColor : 'rgba(26, 26, 26, 0.2)';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    player.draw();
    particles.forEach((p, i) => p.alpha <= 0 ? particles.splice(i, 1) : p.update());
    projectiles.forEach((p, i) => p.alpha <= 0 ? projectiles.splice(i, 1) : p.update());
    enemies.forEach((enemy, eIndex) => {
    enemy.update();
    if (Math.hypot(player.x - enemy.x, player.y - enemy.y) - enemy.radius - player.radius < 1)
    {
        createExplosion(enemy.x, enemy.y, 'white', enemy.radius);
        enemies.splice(eIndex, 1); player.health--; playSound('damage');
        if (player.health <= 0) endGame();
    }
    projectiles.forEach((proj, pIndex) => {
    if (Math.hypot(proj.x - enemy.x, proj.y - enemy.y) - enemy.radius - proj.radius < 1)
    {
        if (Math.random() < currentDifficulty.coinChance) awardCoins(currentDifficulty.coinAmount, proj.x, proj.y);

        if (playerData.equippedAbility === 'multi_fingers')
        {
            for (let i = 0; i < 3; i++)
            {
                const angle = Math.random() * Math.PI * 2;
                const velocity = { x: Math.cos(angle) * 8, y: Math.sin(angle) * 8 };
projectiles.push(new Projectile(proj.x, proj.y, 4, 'yellow', velocity));
                            }
                        }

                        createExplosion(enemy.x, enemy.y, enemy.color, enemy.radius);
score += 10; updateScoreUI(); playSound('hit');
if (gameMode === 'levels' && score >= levelUpScore) levelUp();
setTimeout(() => { enemies.splice(eIndex, 1); projectiles.splice(pIndex, 1); }, 0);
                    }
                });
            });
        }
        function createExplosion(x, y, color, radius)
{
    const isRetro = playerData.equippedItem === 'retro_style';
    const isCartoon = playerData.equippedItem === 'cartoon_style';
    for (let i = 0; i < radius * 2; i++)
    {
        particles.push(new Particle(x, y, Math.random() * (isRetro ? 8 : 4) + (isCartoon ? 10 : 2), color, { x: (Math.random() - 0.5) * 6, y: (Math.random() - 0.5) * 6 }));
            }
        }
        function startLevelsMode()
{
    gameMode = 'levels'; currentLevel = 1; levelUpScore = 150;
    document.getElementById('level-display').style.display = 'flex';
    updateLevelUI();
    currentDifficulty = difficulties.normal;
    startGame();
}
function startDifficultyMode(difficulty)
{
    gameMode = 'difficulty'; lastDifficulty = difficulty;
    currentDifficulty = difficulties[difficulty];
    document.getElementById('level-display').style.display = 'none';
    startGame();
}
function startGame()
{
    showScreen(null);
    document.getElementById('coin-display-main').style.display = 'none';
    gameUI.style.opacity = '1';
    canvas.style.backgroundColor = (gameMode === 'difficulty' && lastDifficulty === 'nightmare') ? currentDifficulty.bgColor : '#1a1a1a';
    resizeCanvas(); init(); animate(); spawnEnemies();
}
function endGame()
{
    cancelAnimationFrame(animationId); clearTimeout(spawnTimerId); animationId = null;
    const lang = playerData.language;
    document.getElementById('final-score').innerHTML = `< span data - key = "your_score" >${ translations[lang].your_score}</ span >: ${ score}`;
showScreen('gameOver'); gameUI.style.opacity = '0'; playSound('gameOver');
        }
        function levelUp()
{
    currentLevel++; levelUpScore = Math.floor(levelUpScore * 1.8 + 100);
    updateLevelUI(); playSound('levelUp');
    if (playerData.equippedItem !== 'retro_style' && playerData.equippedItem !== 'cartoon_style')
    {
        canvas.style.transition = 'box-shadow 0.1s ease-in-out';
        canvas.style.boxShadow = '0 0 50px rgba(255, 255, 100, 0.8)';
        setTimeout(() => canvas.style.boxShadow = '0 0 30px rgba(0, 255, 255, 0.2)', 200);
    }
}
function showScreen(screenId)
{
    Object.keys(screens).forEach(key => screens[key].classList.add('hidden'));
    if (screenId)
    {
        screens[screenId].classList.remove('hidden');
        document.getElementById('coin-display-main').style.display = 'block';
    }
}
function updateScoreUI() { document.getElementById('score-display').innerHTML = `< span data - key = "score" >${ translations[playerData.language].score}</ span >: ${ score}`; }
        function updateLevelUI() { document.getElementById('level-display').innerHTML = `< span data - key = "level" >${ translations[playerData.language].level}</ span >: ${ currentLevel}`; }
        function handleInteraction(event)
{
    if (!animationId) return;
            //if (playerData.equippedAbility === 'multi_fingers') return; // REMOVED to re-enable shooting
            event.preventDefault();
    const rect = canvas.getBoundingClientRect();
    const clientX = event.touches ? event.touches[0].clientX : event.clientX;
            const clientY = event.touches ? event.touches[0].clientY : event.clientY;
            const angle = Math.atan2((clientY - rect.top) - player.y, (clientX - rect.left) - player.x);
            const velocity = { x: Math.cos(angle) * 10, y: Math.sin(angle) * 10 };
            const isRetro = playerData.equippedItem === 'retro_style';
            const isCartoon = playerData.equippedItem === 'cartoon_style';
            const projectileRadius = isRetro ? 6 : (isCartoon ? 5 : 4);
projectiles.push(new Projectile(player.x, player.y, projectileRadius, 'white', velocity));
if (playerData.equippedAbility === 'mirrored_shooting')
{
    projectiles.push(new Projectile(player.x, player.y, projectileRadius, 'white', { x: -velocity.x, y: -velocity.y }));
}
if (isCartoon) { particles.push(new Particle(player.x, player.y, 20, 'yellow', { }, 'shoot_effect')); }
playSound('shoot');
        }
        function applyCanvasAntiAliasing()
{
    const isRetro = playerData.equippedItem === 'retro_style';
    ctx.imageSmoothingEnabled = !isRetro;
}
function resizeCanvas()
{
    const size = Math.min(window.innerWidth, window.innerHeight) * 0.9;
    canvas.width = size; canvas.height = size;
    applyCanvasAntiAliasing();
    if (animationId) init();
}
window.addEventListener('resize', resizeCanvas);
canvas.addEventListener('click', handleInteraction);
canvas.addEventListener('touchstart', handleInteraction);

loadData();
resizeCanvas();
showScreen('intro');
    </ script >
</ body >
</ html >


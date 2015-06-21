local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

speak(hero1, "The turbines are destroyed.")
speak("Aven", "Good. But I'm worried about Zkauba... He hasn't reported back.")
speak(hero2, "Should we follow him to the power plant?")
speak("Aven", "Good idea. Be careful though... The prince hasn't been seen around for a while.")
speak("Aven", "Take this monorail pass to get there.")

playSound('get')
addItem('key_railCard')
speak("Received the RAIL CARD.")

sceneSwitch('aven04', true)

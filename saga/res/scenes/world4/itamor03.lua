local hero1 = getHero(0).getName()
local hero4 = getHero(4).getName()

speak("Itamor", "Thank you for all your help.")
speak(hero4, "It was our pleasure.")
speak("Itamor", "Please come see me in the seer's house any time.")

walk('itamor', 9, 'NORTH')
sceneSwitch('itamor03', true)

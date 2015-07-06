local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()

playBGM('ffl3_steslos')
pathEvent('hero', 'target_hero')

speak("Jonas", "Give up the sword, Janine. MURAMAS is cursed! Let Aven destroy it.")
speak("Aven", "The legendary swords are tools of the masters. They only bring destruction.")
speak("Janine", "I'll never give it up! I'll use this sword and fight my way to Babel's heart.")
speak("Aven", "Babel must be sealed, not opened.")
speak("Janine", "I care about defeating the masters, not sealing worlds. And I'll do whatever it takes!")

pathEvent('janine', 'target_middle')
pathEvent('janine', 'gate')

face('jonas', 'SOUTH')
face('aven', 'SOUTH')
face('zkauba', 'SOUTH')
face('lara', 'SOUTH')

speak("Jonas", hero1 .. ", I hear you managed to defeat Ares.")
speak(hero1, "We couldn't have done it without Janine.")
speak("Jonas", "It's an empty victory if we can't seal the gates!")
speak("Aven", "The legendary swords can easily open my seals. They must be destroyed.")
speak(hero1, "I don't know...")
speak("Aven", "The Brotherhood will return to our world and research the swords.")
speak("Jonas", "I'll do my best to track down Janine.")
speak(hero1, "What about the prince?")
speak("Aven", "We can't challenge him yet. The seals come first.")
speak("Aven", "I'm sure we'll meet again.")

playBGM('ffl2_apollo')

walk('aven', 2, 'WEST', false)
walk('jonas', 2, 'WEST', false)
walk('zkauba', 2, 'WEST', false)
walk('lara', 2, 'WEST', true)

walk('aven', 7, 'NORTH', false)
walk('jonas', 7, 'NORTH', false)
walk('zkauba', 7, 'NORTH', false)
walk('lara', 7, 'NORTH', true)

sceneSwitch('twins03', true)

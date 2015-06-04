local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

speak(hero1, "How is Jonas?")
speak("Lara", "It'll take time, but he's in good care.")
speak(hero2, "What should we do now?")
speak("Lara", "Jonas and I are both members of a brotherhood. We've been working to seal off Babel from the likes of the masters.")
speak("Lara", "Maybe you've heard about the seal on Hero? Similar seals are on most gates that connect to Babel.")
speak(hero1, "And the masters want to open these gates?")
speak("Lara", "I think so, anyway. Jonas and I are both out of action, but I trust you can find the rest of the brotherhood.")
speak("Lara", "The seal on this world may be damaged. I would investigate.")
speak(hero1, "We will go, then.")
speak("Lara", "Good luck! The masters bring only destruction and they must be stopped!")
sceneSwitch('lara03', true)
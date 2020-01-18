local hero1 = getHero(0).getName()
local hero2 = getHero(2).getName()

speak("Lara", "We're coming up on the lands of the world worm.")
speak(hero2, "Should we fear the worm?")
speak("Lara", "As much as the wolf. If I'm right, he could be one of the masters of Babel...")
speak(hero1, "A master?")
speak("Lara", "My boss doesn't want me to say it everywhere, but these 'masters' seem to be using Babel to punch holes to other worlds.")
speak(hero1, "Like the gate at Hero!")
speak("Lara", "And here as well. When Jonas is free we should be able to talk more.")

sceneSwitch('lara02', true)

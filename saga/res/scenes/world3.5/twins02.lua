local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

speak("BORG", "Janine!")
speak("Janine", hero1 .. ", you fixed up BORG?")
speak(hero1, "It was thanks to Elder.")
speak("BORG", "I'm glad you're safe. It's very dangerous here!")
speak("Janine", "I know. I can't take Ares and his guards head on, even if I do have MURAMAS. I need a distraction.")
speak(hero2, "We can hold Ares off if you need.")
speak("Janine", "Good idea! " .. hero1 .. ", you guys go to Ares's throne room. BORG and I will set an ambush.")
speak(hero1, "We'll do it.")

sceneSwitch('borg_leaves_again', true)
removeMember()
walk('borg', 1, 'EAST')
face('borg', 'WEST')
face('hero', 'EAST')

speak("BORG", "Be careful!")

walk('janine', 1, 'SOUTH')

pathEvent('borg', 'corner1', false)
pathEvent('janine', 'corner2', true)
walk('borg', 1, 'NORTH', false)
walk('janine', 1, 'EAST', true)
pathEvent('borg', 'target_tele', false)
pathEvent('janine', 'target_tele', true)

sceneSwitch('twins02', true)

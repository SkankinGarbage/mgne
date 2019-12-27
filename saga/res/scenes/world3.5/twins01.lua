local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()

playBGM('twins_theme')
speak(hero3, "It's the twins!")
wait(0.7)

speak("Jonas", "You traitor! If it weren't for you, the prince wouldn't have been able to open the gate in the first place!")
speak("Janine", "I'm the one trying to stop the masters, here! I have the sword! Just help me get to Ares!")
speak("Jonas", "Look what happened when we tried that before! We lost BORG. And now you won't help me find his ROM?")
speak("Janine", "Jonas, we're running out of time. If Ares comes back and we can't pierce his shield, we're doomed.")
speak("Jonas", "How could you? BORG raised us! I'll do what I think is right.")

walk('hero', 1, 'NORTH')
wait(0.3)
face('jonas', 'SOUTH')
face('janine', 'SOUTH')

speak(hero1, "Jonas... Janine...")

speak("Jonas", hero1 .. ", I never got a chance to thank you. And I wish I had time for a proper greeting, but...")
speak("Jonas", "I've got to save BORG before we lose him forever!")

pathEvent('jonas', 'jonas_target1')
face('jonas', 'SOUTH')
speak("Jonas", "I'll see you again soon.")
walk('jonas', 2, 'SOUTH')
face('hero', 'SOUTH')
pathEvent('jonas', 'jonas_target2')

face('hero', 'NORTH')
pathEvent('janine', 'janine_target1')
face('janine', 'SOUTH')
speak("Janine", hero1 .. ", I'm sorry Jonas isn't much use, but I'm glad you're here. I have a plan to defeat Ares.")
speak(hero1, "How?")
speak("Janine", "This is MASAMUNE, one of the four legendary swords. It should be able to pierce his shield. Now instead of running, we can fight!")
speak("Janine", "But I can't stand against Ares alone. Maybe I'll see you at his camp?")
walk('janine', 2, 'SOUTH')
face('hero', 'SOUTH')
pathEvent('janine', 'janine_target2')

playBGM('home_town')
wait(0.7)

speak(hero1, "How could we pick sides?")
speak(hero2, "Maybe we should talk to Elder.")

sceneSwitch('twins01', true)

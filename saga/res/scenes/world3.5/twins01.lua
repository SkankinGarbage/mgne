local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()

playBGM('ffl3_steslos')
speak(hero3, "It's the twins!")
wait(0.7)

speak("Jonas", "You traitor! If it weren't for you, the prince wouldn't have been able to open the gate in the first place!")
speak("Janine", "I'm the one trying to stop the masters, here! I have the sword! Just help me get the tablet.")
speak("Jonas", "Look what happened when you tried that! We lost BORG. And now you won't help me find his ROM?")
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
speak("Janine", "This is MURAMAS, one of the four legendary swords. It could break Ares's shield, but it's cursed.")
speak("Janine", "All I need is a tablet to break the curse. I know there's one at the southern cave. You have to help!")
walk('janine', 2, 'SOUTH')
face('hero', 'SOUTH')
pathEvent('janine', 'janine_target2')

playBGM('ffl2_hometown')
wait(0.7)

speak(hero1, "How could we pick sides?")
speak(hero2, "Maybe we should talk to Elder.")


sceneSwitch('twins01', true)

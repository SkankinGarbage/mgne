local hero1 = getHero(0).getName()
local hero2 = getHero(3).getName()

speak("Man", "Hmmm... You must be those rebels we've been warned about.")
speak("Jonas", "Rebels?")
speak("Man", "The Brotherhood. You might not realize, but Moloch's very worried. That's why he's after mystic swords.")
speak(hero1, "Who are you?")
speak("Man", "I remember a time before the poison wastes. There was no Moloch. We don't need him. Maybe my information can be useful.")
speak("Man", "You can move through the southern tower via warp squares. There's one hidden at the westernmost point of the ground floor.")
speak(hero2, "Thanks for your help!")

sceneSwitch('slave01', true)

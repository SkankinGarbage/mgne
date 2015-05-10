local hero1 = getHero(0).getName()
local hero2 = getHero(2).getName()

speak("Grandmaster", "Welcome to the Castle of Hero. None have passed through in a very long time.")
speak(hero1, "There was a strange man guarding the door who talked about mysterious 'masters.' Could he have been stopping passers through?")
speak("Grandmaster", "Masters? He must be with the enemy.")
speak(hero2, "Well, he is dead now. Who is the enemy?")
speak("Grandmaster", "The Knights of Hero fight to keep sealed the monster gate, in the middle of the city.")
speak("Grandmaster", "But monsters have been slipping through... and some force must be behind this.")
speak("Grandmaster", "Will you help us in our mission, and become knights yourselves?")
speak("David", "You have my word!")
speak(hero1, "I'm sorry, but we have another mission as well. I am looking for family.")
speak("Grandmaster", "Well, you may stay in our castle. If you change your mind, there have been reports of a similar enemy hiding in the Oracle's Shrine east of the castle. Any help is welcome.")
sceneSwitch('entry_grandmaster01', true)

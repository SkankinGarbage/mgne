local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

speak("Lara", "Where is the wolf?")
speak("Steward", "The world wolf is not here. But I will slay you in his name, traitors!")

battle('party_bossSteward', false)
sceneSwitch('steward_dead', true)
wait(0.7)

speak(hero2, "Then Jonas really must be held by the worm.")
speak("Lara", "I'll come with you to the worm's castle, if you can find a way.")
speak("Lara", "Jonas was working for my organization when he was captured. We believe that the wolf and worm may be from a hidden world called Babel.")
speak(hero1, "Babel? We've been there, but what is it? What are you and Jonas up to?")
speak("Lara", "Speak no more! We have work to do.")

local hero1 = getHero(0).getName()

speak("RADIO", "This is Jonas! I've got news.")
speak(hero1, "So do we. We have two parts of EXCALIBUR now.")
speak("Jonas", "Good work. Then the only other piece is being held by Moloch...")
speak("Jonas", "Anyway, meet me at the underground city. We can talk more in person!")

sceneSwitch('jonas_radio02', true)

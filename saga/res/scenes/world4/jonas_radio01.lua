local hero1 = getHero(0).getName()

speak("RADIO", "This is Jonas! " .. hero1 .. ", any news?")
speak(hero1, "Jonas? We've found one part of EXCALIBER.")
speak("Jonas", "Good work! I'm exploring the ruins of the grand city.")
speak("Jonas", "According to this library, there's special armor that grants immunity to the poison wastes...")
speak(hero1, "Where is it?")
speak("Jonas", "No idea. Keep looking for more parts of EXCALIBER. There should be at least one more out there!")

sceneSwitch('jonas_radio01', true)

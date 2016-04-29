local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

sceneSwitch('temple_jonas', true)

walk('jonas', 9, 'SOUTH')
face('hero', 'EAST')
walk('jonas', 1, 'WEST')

speak("Jonas", hero1 .. "! You beat me here, but I think we're too late.")
speak(hero1, "Jonas! You found this world? And even this temple?")
speak("Jonas", "I came here following Janine's trail. I think Janine, the prince, and this world's master Moloch are after EXCALIBUR.")
speak("Jonas", "The monks here kept a part so I was wondering if they'd seen her, but...")
speak(hero3, "The master's men got here first.")
speak("Jonas", "We're too late to help the monks, but at least we met up. Aven said my priority is to secure EXCALIBUR, even more than finding Janine.")
speak("Jonas", "Will you help me and the brotherhood? Don't tell me Janine already got to you.")
speak(hero4, "Well...")
speak("Jonas", "It doesn't matter. Let's work together if we can.")
speak("Jonas", "Now that we know Moloch has at least one part of EXCALIBUR, we need to find a way through the wastes and into his lair.")
speak(hero1, "How can we help?")
speak("Jonas", "I'll look for a way to Moloch's base. You try to find the other two pieces.")
speak("Jonas", "Here, why don't you take this?")

playSound('get')
speak("Received the RADIO.")

speak(hero1, "A sending stone again?")
speak("Jonas", "Yep! I'll let you know if I find anything. Keep me up to date, okay?")
speak(hero1, "It's a plan.")
speak("Jonas", "And I'm glad we're on the same side again.")

walk('jonas', 1, 'EAST')
walk('jonas', 9, 'NORTH')

sceneSwitch('temple_jonas', false)
sceneSwitch('jonas01', true)

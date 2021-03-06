local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

playSound('get')
sceneSwitch('tower02', true)
speak("Retrieved part 3 of $EEXCALIBUR.")
wait(0.8)

sceneSwitch('jonas_tower', true)
face('hero', 'SOUTH')
walk('jonas', 1, 'SOUTH')
face('jonas', 'NORTH')

speak("Jonas", "This is it! We finally have all the parts.")
speak(hero2, "Now to reforge EXCALIBUR?")
speak("Jonas", "What? That's what we're trying to stop!")
speak(hero1, "Then how are we supposed to stop Moloch?")
speak("Jonas", "We don't have to. I'll just take this part back to Aven, and he'll seal the gate.")
speak(hero4, "That's terrible! We can't just let the masters rule this world!")
speak(hero1, "And isn't Janine on this world somewhere? You'd seal her here?")
speak("Jonas", "It's all according to Aven's plan. We have to protect Babel. I'm headed back there now.")
speak(hero3, "Traitor!")
speak("Jonas", hero1 .. "...")
speak(hero1, "Then we'll fight Moloch without you and without EXCALIBUR.")
speak("Jonas", "Pah! You're just like Janine! Come follow me or you'll be killed up there.")

walk('jonas', 3, 'SOUTH')
walk('jonas', 6, 'EAST')
playSound('rebound')
sceneSwitch('jonas_tower', false)
removeMember()

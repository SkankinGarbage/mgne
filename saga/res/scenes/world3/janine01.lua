local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

walk('hero', 'EAST', 2)
walk('janine', 'EAST', 2)
walk('janine', 'SOUTH', 2)
walk('janine', 'WEST', 2)

speak("Janine", "Could that be... " .. hero1 .. "?")
speak(hero1, "Janine! What are you doing here?")
speak("Zkauba", "You know this woman? She's a minion of the prince! She designed the gate machine!")
speak(hero1, "You've sided with the masters?")
speak("Janine", "Never! It's a long story, but...")
speak("Janine", "The only way to defeat the masters is to use the power within Babel. So we must open the gates! It's the only way!")
speak("Zkauba", "Mortals can never hope to defeat the masters. Aven says sealing them is the only way.")
speak("Janine", "We can't give up, either! I won't let the prince eat this world.")
speak("Zkauba", "Then why do you help him?")
speak("????", "They went this way!")
speak("Zkauba", "Guards! Quick, let's kidnap this scientist and go.")
speak("Janine", hero1 .. ", we'll meet again!")

walk('janine', 'WEST', 3)
face('hero', 'WEST')
walk('janine', 'WEST', 3)

wait(0.5)

walk('hero', 'EAST', 3)
face('hero', 'WEST')

sceneSwitch('lab_guards', true)
walk('guard1', 'EAST', 3, false)
walk('guard2', 'EAST', 3, false)
walk('guard3', 'EAST', 3, true)
walk('guard1', 'SOUTH', 1, false)
walk('guard2', 'EAST', 1, false)
walk('guard3', 'EAST', 1, true)
walk('guard2', 'NORTH', 1, false)
walk('guard3', 'EAST', 1, true)

speak("Guard", "The Brotherhood! Men, arrest these scums!")

fade('white')
wait(1.0)
sceneSwitch('janine01', true)
sceneSwitch('lab_guards', false)
-- teleport('world2/coast_town_interior.tmx', 65, 26, 'NORTH', false)
fade('normal')
wait(1.0)

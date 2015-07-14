local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()

speak(hero1, "Urgh...")
speak(hero2, "My skin is almost boiling. It feels really unhealthy here and it just keeps getting worse.")
speak(hero3, "We'd better stay away.")

walk('hero', 1, 'NORTH')

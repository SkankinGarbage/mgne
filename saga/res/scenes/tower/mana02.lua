local hero1 = getHero(0).getName()

speak(hero1, "We have the stones.")
speak("???", "Excellent. Now lost magic shall be yours!")

removeItem('stone_fire', true)
removeItem('stone_water', true)
removeItem('stone_earth', true)
removeItem('stone_air', true)
sceneSwitch('mana02', true)

mix('mixset_magic')

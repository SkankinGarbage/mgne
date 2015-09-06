walk('guard_l2', 'WEST', 1, false)
walk('guard_r2', 'EAST', 1)
face('guard_l2', 'NORTH')
face('guard_r2', 'NORTH')

sceneSwitch('missile_trap02_01', true)
shake(0.5, 4)
wait(0.5)
sceneSwitch('missile_trap02_02', true)
shake(0.5, 4)
wait(0.5)

sceneSwitch('missile_trap02', true)

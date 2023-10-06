# Minecraft Java Edition Unity (not Minecraft!)
# Connection and blockID : MCJE
# World parameters : Unity

print("param_UNITY loaded")

# axis parameters
AXIS_WIDTH = 100       # x, z: -100 .. 0 .. 100
AXIS_TOP = 127
AXIS_Y_V_ORG = 64     # y of virtual origin
AXIS_BOTTOM = 0      # y: 0 .. 64 .. 127

# virtical levels for Uniyt
Y_TOP = 255           # the top where blocks can be set
Y_CLOUD_BOTTOM = 128  # the bottom of clouds
Y_SEA = -1            # the sea level
Y_BOTTOM = 0          # no use. the bottom where blocks can be set
Y_BOTTOM_STEVE = -64  # no use. the bottom where Steve can go down

# connection port
PORT_MC = 14712  # Unity

# block IDs  You can find IDs here: https://minecraft-ids.grahamedgecombe.com/
AIR = "air"
STONE = "stone"
GRASS_BLOCK = "grass_block"
GOLD_BLOCK = "gold_block"
IRON_BLOCK = "iron_block"
GLOWSTONE = "glowstone"
SEA_LANTERN_BLOCK = "sea_lantern"

# some good blocks for grid like patterns you can count blocks easily
GLASS = "glass"
TNT = "tnt"
DIAMOND_BLOCK = "diamond_block"
FURNACE_INACTIVE = "furnace"
FURNACE_ACTIVE = "lit_furnace"
GLASS_PANE = "glass_pane"

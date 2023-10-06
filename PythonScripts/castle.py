from mcje.minecraft import Minecraft

# import param_MCJE as param
import param_UNITY as param

# from time import sleep

mc = Minecraft.create(port=param.PORT_MC)
mc.postToChat("Let's build a castle!")


def CreateWalls(x, y, z, size, height, material, battlements, walkway):
    # Create 4 walls with a specified size, height and material.
    # Battlements and walkways can also be added to the top edges.

    mc.setBlocks(x - size, y + 1, z - size, x + size, y + height, z - size, material)
    mc.setBlocks(x - size, y + 1, z - size, x - size, y + height, z + size, material)
    mc.setBlocks(x + size, y + 1, z + size, x - size, y + height, z + size, material)
    mc.setBlocks(x + size, y + 1, z + size, x + size, y + height, z - size, material)

    # Add battlements to top edge
    if battlements:
        for q in range(0, (2 * size) + 1, 2):
            mc.setBlock(x - size, y + height + 1, z - size + q, material)
            mc.setBlock(x + size, y + height + 1, z - size + q, material)
            mc.setBlock(x - size + q, y + height + 1, z + size, material)
            mc.setBlock(x - size + q, y + height + 1, z - size, material)

    # Add wooden walkways
    if walkway:
        mc.setBlocks(
            x - size + 1,
            y + height - 1,
            z + size - 1,
            x + size - 1,
            y + height - 1,
            z + size - 1,
            param.WOOD_PLANKS,
        )
        mc.setBlocks(
            x - size + 1,
            y + height - 1,
            z - size + 1,
            x + size - 1,
            y + height - 1,
            z - size + 1,
            param.WOOD_PLANKS,
        )
        mc.setBlocks(
            x - size + 1,
            y + height - 1,
            z - size + 1,
            x - size + 1,
            y + height - 1,
            z + size - 1,
            param.WOOD_PLANKS,
        )
        mc.setBlocks(
            x + size - 1,
            y + height - 1,
            z - size + 1,
            x + size - 1,
            y + height - 1,
            z + size - 1,
            param.WOOD_PLANKS,
        )


def CreateLandscape(x, y, z, islandwidth, moatwidth, moatdepth):
    totalSize = islandwidth + moatwidth + 2

    # Set upper half to air
    mc.setBlocks(
        x - totalSize,
        y,
        z - totalSize,
        x + totalSize,
        y + 100,
        z + totalSize,
        param.AIR,
    )
    # Create square of grass
    mc.setBlocks(
        x - totalSize,
        y,
        z - totalSize,
        x + totalSize,
        y - 1,
        z + totalSize,
        param.GRASS,
    )
    # with a block of dirt underneath it
    mc.setBlocks(
        x - totalSize,
        y - 1,
        z - totalSize,
        x + totalSize,
        y - moatdepth - 1,
        z + totalSize,
        param.DIRT,
    )
    # Create water moat
    mc.setBlocks(
        x - islandwidth - moatwidth,
        y,
        z - islandwidth - moatwidth,
        x + islandwidth + moatwidth,
        y - moatdepth,
        z + islandwidth + moatwidth,
        param.WATER,
    )
    # Create island
    mc.setBlocks(
        x - islandwidth,
        y,
        z - islandwidth,
        x + islandwidth,
        y - moatdepth,
        z + islandwidth,
        param.GRASS,
    )


def CreateKeep(x, y, z, size, floors):
    # Create a keep with a specified number
    # of floors floors and a roof
    height = (floors * 5) + 5

    mc.postToChat("  Creating walls ...")
    CreateWalls(x, y, z, size, height, param.STONE_BRICK, True, True)

    # Floors & Windows
    mc.postToChat("  Creating floors ...")
    for floor in range(1, floors + 1):
        mc.setBlocks(
            x - size + 1,
            (floor * 5) + y,
            z - size + 1,
            x + size - 1,
            y + (floor * 5),
            z + size - 1,
            param.WOOD_PLANKS,
        )

    # Staircase holes in floors
    mc.postToChat("  Creating stairs ...")
    mc.setBlocks(
        x - size + 1,
        y + 1,
        z - size + 1,
        x - size + 1,
        y + (floors * 5),
        z - size + 3,
        param.AIR,
    )
    # Stairs
    for floor in range(1, floors + 1):
        print("Stairs for floor ", floor)
        mc.setBlock(x - size + 1, (floor * 5) + y - 1, z - size + 1, param.WOOD_PLANKS)
        mc.setBlock(x - size + 1, (floor * 5) + y - 2, z - size + 2, param.WOOD_PLANKS)
        mc.setBlock(x - size + 1, (floor * 5) + y - 3, z - size + 3, param.WOOD_PLANKS)
        mc.setBlock(x - size + 1, (floor * 5) + y - 4, z - size + 4, param.WOOD_PLANKS)
        mc.setBlock(x - size + 1, (floor * 5) + y - 2, z - size + 5, param.TORCH)

    # Windows
    mc.postToChat("  Creating windows ...")
    for floor in range(1, floors + 1):
        CreateWindows(x, (floor * 5) + y + 2, z + size, "N")
        CreateWindows(x, (floor * 5) + y + 2, z - size, "S")
        CreateWindows(x - size, (floor * 5) + y + 2, z, "W")
        CreateWindows(x + size, (floor * 5) + y + 2, z, "E")

    # Door
    mc.setBlocks(x, y + 1, z - size, x, y + 2, z - size, param.AIR)


def CreateWindows(x, y, z, dir):
    if dir == "N" or dir == "S":
        z1 = z
        z2 = z
        x1 = x - 2
        x2 = x + 2

    if dir == "E" or dir == "W":
        z1 = z - 2
        z2 = z + 2
        x1 = x
        x2 = x

    mc.setBlocks(x1, y, z1, x1, y + 1, z1, param.AIR)
    mc.setBlocks(x2, y, z2, x2, y + 1, z2, param.AIR)

    if dir == "N":
        a = 3
    if dir == "S":
        a = 2
    if dir == "W":
        a = 0
    if dir == "E":
        a = 1


keepFloors = 4
keepSize = 5

outerWallSize = 21
outerWallHeight = 5

innerWallSize = 13
innerWallHeight = 6

moatDepth = 5
moatWidth = 5

# Get the position of the player
x, y, z = 0, 0, 0

mc.postToChat("Creating ground and moat ...")
print("Create ground and moat")
CreateLandscape(x, y, z, outerWallSize + 2, moatWidth, moatDepth)

mc.postToChat("Creating outer walls ...")
print("Create outer walls")
CreateWalls(x, y, z, outerWallSize, outerWallHeight, param.STONE_BRICK, True, True)

mc.postToChat("Creating inner walls ...")
print("Create inner walls")
CreateWalls(x, y, z, innerWallSize, innerWallHeight + 1, param.STONE_BRICK, True, True)

mc.postToChat("Creating keep ...")
print("Create Keep with 4 levels")
CreateKeep(x, y, z, keepSize, keepFloors)


mc.postToChat("Finished!")

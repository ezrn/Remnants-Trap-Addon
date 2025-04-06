using Microsoft.Xna.Framework;
using System;
using System.Numerics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EzrnMoreTraps.WorldGen
{
    public class AddTraps
    {
        private const int minDistance = 3;
        private const int maxDistance = 75;

        public void PlaceTraps(float frequency)
        {
            int trapGoal = (int)(3000 * frequency);
            int placed = 0;
            int attempts = 0;
            int maxAttempts = trapGoal * 20;

            while (placed < trapGoal && attempts < maxAttempts)
            {
                attempts++;

                (int startX, int startY) = GetRandomCavernOrUndergroundSpot();

                // find valid plate location
                (int plateX, int plateY) = FindAirAboveSolid(startX, startY, 50);
                if (plateX == -1)
                    continue;

                // check if the candidate is NOT a pressure plate rail
                if (!IsPressurePlateRail(plateX, plateY))
                {
                    if (!PlacePressurePlateAtCandidate(plateX, plateY))
                        continue;
                }

                // kill trap placement if there isnt enough vertical clearance
                (int ceilingX, int ceilingY) = FindAirBelowSolid(plateX, plateY, 50);
                if (ceilingX == -1 || (plateY - ceilingY) < minDistance)
                {
                    Terraria.WorldGen.KillTile(plateX, plateY, noItem: true);
                    continue;
                }

                // randomly choose trap type
                bool useRegularTrap = true;
                if (ModContent.GetInstance<EzrnMoreTrapsConfig>().UseSpikyBallTraps)
                {
                    useRegularTrap = (Terraria.WorldGen.genRand.Next(2) == 0);
                }

                bool success = false;
                if (useRegularTrap)
                {
                    success = TryPlaceFlameOrDartTrap(plateX, plateY);
                }
                else
                {
                    success = SetupSpikyBallTrap(plateX, plateY);
                }

                if (success)
                {
                    placed++;
                }
                else
                {
                    // remove pressure plate on failiure
                    Terraria.WorldGen.KillTile(plateX, plateY, noItem: true);
                }
            }
        }

        private bool IsPressurePlateRail(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            if (tile.HasTile && tile.TileType == 314)
            {
                // determine style by tile.TileFrameX / 18.
                int style = tile.TileFrameX / 18;
                if (style == 1)
                    return true;
            }
            return false;
        }


        public bool PlacePressurePlateAtCandidate(int plateX, int plateY)
        {
            Terraria.WorldGen.KillTile(plateX, plateY, noItem: true);

            int supportTileType = (plateY > Main.rockLayer) ? TileID.Stone : TileID.Dirt;

            int supportY = plateY + 1;
            Terraria.WorldGen.KillTile(plateX, supportY, noItem: true);

            Terraria.WorldGen.PlaceTile(plateX, supportY, supportTileType, forced: true, style: 0);

            bool placed = Terraria.WorldGen.PlaceTile(plateX, plateY, TileID.PressurePlates, forced: true, style: 2);

            return placed;
        }

        public bool SetupSpikyBallTrap(int plateX, int plateY)
        {
            (int trapX, int trapY) = FindAirBelowSolid(plateX, plateY, 50);
            if (trapX == -1 || trapY <= 0 || (plateY - trapY) < minDistance)
            {
                return false;
            }
            // move into the ceiling one tile for more natural look
            trapY = trapY - 1;
            if (trapY < 0)
                return false;

            int spikyBallTileID = ModContent.TileType<Tiles.SpikyBallTrapLowTier>();

            // place the trap
            Terraria.WorldGen.KillTile(trapX, trapY, noItem: true);
            Terraria.WorldGen.PlaceTile(trapX, trapY, spikyBallTileID, forced: true);
            WireHorizontalAndVertical(plateX, plateY, trapX, trapY);
            // clear area below trap
            ClearAreaBelowTrap(trapX, trapY, plateY);

            // chance to add a second trap
            if (Terraria.WorldGen.genRand.Next(2) == 0)
            {
                int extraRightX = trapX + 1;
                if (extraRightX < Main.maxTilesX)
                {
                    Terraria.WorldGen.KillTile(extraRightX, trapY, noItem: true);
                    Terraria.WorldGen.PlaceTile(extraRightX, trapY, spikyBallTileID, forced: true);
                    ClearAreaBelowTrap(extraRightX, trapY, plateY);
                    //wire traps
                    Framing.GetTileSafely(extraRightX, trapY).RedWire = true;
                    Terraria.WorldGen.SquareTileFrame(extraRightX, trapY);
                }
            }
            // another chance to add a third trap
            if (Terraria.WorldGen.genRand.Next(2) == 0)
            {
                int extraLeftX = trapX - 1;
                if (extraLeftX >= 0)
                {
                    Terraria.WorldGen.KillTile(extraLeftX, trapY, noItem: true);
                    Terraria.WorldGen.PlaceTile(extraLeftX, trapY, spikyBallTileID, forced: true);
                    ClearAreaBelowTrap(extraLeftX, trapY, plateY - 2);
                    Framing.GetTileSafely(extraLeftX, trapY).RedWire = true;
                    Terraria.WorldGen.SquareTileFrame(extraLeftX, trapY);
                }
            }
            return true;
        }

        //used for spiky ball traps to assure they dont drop the balls into the ceiling blocks
        private void ClearAreaBelowTrap(int x, int startY, int endY) 
        {
            for (int y = startY + 1; y < endY; y++)
            {
                if (y < 0 || y >= Main.maxTilesY)
                    break;
                Terraria.WorldGen.KillTile(x, y, noItem: true);
            }
        }

        public bool TryPlaceFlameOrDartTrap(int plateX, int plateY)
        {
            bool isRail = IsPressurePlateRail(plateX, plateY);

            int offset = isRail ? 0 : Terraria.WorldGen.genRand.Next(0, 3); // for rail mode, force offset = 0(on the rail y level)
            int trapY = plateY - offset;
            if (trapY < 0)
                return false;

            // choose random direction
            int direction = (Terraria.WorldGen.genRand.Next(2) == 0) ? -1 : 1;
            (int candidateX, int horizDistance) = ScanForCandidate(plateX, trapY, direction);
            // try opposite side
            if (horizDistance < minDistance || horizDistance == maxDistance)
            {
                direction *= -1;
                (int candidateXAlt, int horizDistanceAlt) = ScanForCandidate(plateX, trapY, direction);
                if (horizDistanceAlt < minDistance)
                    return false;
                candidateX = candidateXAlt;
                horizDistance = horizDistanceAlt;
            }

            // move into wall optionally
            if (!isRail && horizDistance < maxDistance)
            {
                candidateX = candidateX + direction;
            }
            if (isRail && horizDistance < maxDistance)
            {
                candidateX = candidateX + direction;
            }

            if (!isRail)// trap picking logic
            {
                bool useFlameTrap = (horizDistance <= 15); //since flame traps have shorter range

                Tile candidateTile = Framing.GetTileSafely(candidateX, trapY);
                if (candidateTile.LiquidAmount > 0 && candidateTile.LiquidType == LiquidID.Water) //dont place flame traps in water
                {
                    useFlameTrap = false;
                }

                if (useFlameTrap)
                {
                    int flameTileID = ModContent.TileType<Tiles.FlameTrapLowTier>();
                    Terraria.WorldGen.KillTile(candidateX, trapY, noItem: true);
                    bool plateIsRight = (plateX > candidateX);
                    Terraria.WorldGen.PlaceTile(candidateX, trapY, flameTileID, forced: true, style: plateIsRight ? 0 : 1);
                    Tiles.FlameTrapLowTier.SetStyle(candidateX, trapY, plateIsRight ? 0 : 1);
                }
                else //dart traps if useFlameTrap is false
                {
                    int dartTileID = ModContent.TileType<Tiles.DartTrapLowTier>();
                    Terraria.WorldGen.KillTile(candidateX, trapY, noItem: true);
                    bool plateIsRight = (plateX > candidateX);
                    Terraria.WorldGen.PlaceTile(candidateX, trapY, dartTileID, forced: true, style: plateIsRight ? 0 : 1);
                    Tiles.DartTrapLowTier.SetStyle(candidateX, trapY, plateIsRight ? 0 : 1);
                }
            }
            else
            {
                //for rails just use 50/50 for trap selection since player is moving constantly
                bool useFlameTrap = (Terraria.WorldGen.genRand.Next(2) == 0);
                if (useFlameTrap)
                {
                    int flameTileID = ModContent.TileType<Tiles.FlameTrapLowTier>();
                    Terraria.WorldGen.KillTile(candidateX, trapY, noItem: true);
                    bool plateIsRight = (plateX > candidateX);
                    Terraria.WorldGen.PlaceTile(candidateX, trapY, flameTileID, forced: true, style: plateIsRight ? 0 : 1);
                    Tiles.FlameTrapLowTier.SetStyle(candidateX, trapY, plateIsRight ? 0 : 1);
                }
                else
                {
                    int dartTileID = ModContent.TileType<Tiles.DartTrapLowTier>();
                    Terraria.WorldGen.KillTile(candidateX, trapY, noItem: true);
                    bool plateIsRight = (plateX > candidateX);
                    Terraria.WorldGen.PlaceTile(candidateX, trapY, dartTileID, forced: true, style: plateIsRight ? 0 : 1);
                    Tiles.DartTrapLowTier.SetStyle(candidateX, trapY, plateIsRight ? 0 : 1);
                }

                TryReplaceRails(candidateX, trapY);
            }

            // support the trap if its floating with a single stone block(rarely happens but looks a bit nicer if it does)
            for (int k = 1; k <= offset; k++)
            {
                int blockY = trapY + k;
                if (blockY >= Main.maxTilesY)
                    break;
                Tile belowTrap = Framing.GetTileSafely(candidateX, blockY);
                if (!belowTrap.HasTile)
                {
                    Terraria.WorldGen.KillTile(candidateX, blockY, noItem: true);
                    Terraria.WorldGen.PlaceTile(candidateX, blockY, TileID.Stone, forced: true);
                }
            }

            WireHorizontalAndVertical(plateX, plateY, candidateX, trapY);
            Terraria.WorldGen.SquareTileFrame(plateX, plateY);
            Terraria.WorldGen.SquareTileFrame(candidateX, trapY);

            return true;
        }

        /*
         * This method removes a 3 wide area of rails around a trap, then places the rails one tile up
         * this essentially allows the traps to be inside of the rail(since remnants rails are srtaight lines)
         */
        private void TryReplaceRails(int x, int y)
        {
            bool replacedAny = false;
            if (IsNormalRail(x, y))
            {
                Terraria.WorldGen.KillTile(x, y, noItem: true);
                replacedAny = true;
            }
            if (x - 1 >= 0 && IsNormalRail(x - 1, y))
            {
                Terraria.WorldGen.KillTile(x - 1, y, noItem: true);
                replacedAny = true;
            }
            if (x + 1 < Main.maxTilesX && IsNormalRail(x + 1, y))
            {
                Terraria.WorldGen.KillTile(x + 1, y, noItem: true);
                replacedAny = true;
            }
            if (replacedAny)
            {
                if (IsNormalRail(x, y) == false)
                {
                    if (y - 1 >= 0)
                        Terraria.WorldGen.PlaceTile(x, y - 1, 314, forced: true, style: 0);
                }
                if (x - 1 >= 0)
                {
                    if (y - 1 >= 0)
                        Terraria.WorldGen.PlaceTile(x - 1, y - 1, 314, forced: true, style: 0);
                }
                if (x + 1 < Main.maxTilesX)
                {
                    if (y - 1 >= 0)
                        Terraria.WorldGen.PlaceTile(x + 1, y - 1, 314, forced: true, style: 0);
                }
            }
        }
        private bool IsNormalRail(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            if (tile.HasTile && tile.TileType == 314)
            {
                int style = tile.TileFrameX / 18;
                return (style == 0);
            }
            return false;
        }

        //returns the location for a valid trap location, useful for other methods
        private (int candidateX, int horizDistance) ScanForCandidate(int plateX, int trapY, int direction)
        {
            int candidateX = -1;
            int distance = 0;
            for (int i = 1; i <= maxDistance; i++)
            {
                int checkX = plateX + (i * direction);
                if (checkX < 10 || checkX >= Main.maxTilesX - 10)
                    break;
                Tile tile = Framing.GetTileSafely(checkX, trapY);
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
                    candidateX = checkX - direction;
                    distance = i - 1;
                    return (candidateX, distance);
                }
            }
            candidateX = plateX + (maxDistance * direction);
            distance = maxDistance;
            return (candidateX, distance);
        }

        //this method is somewhat faulty, allowing for "buried" pressure plates
        //finds the first air block below a non-air block(logically this was necessary in some circumstances)
        //vs just using solid vs non solid
        public (int x, int y) FindAirBelowSolid(int startX, int startY, int maxUp)
        {
            for (int offset = 1; offset <= maxUp; offset++)
            {
                int checkY = startY - offset;
                if (checkY < 1)
                    break;
                Tile above = Framing.GetTileSafely(startX, checkY - 1);
                Tile current = Framing.GetTileSafely(startX, checkY);
                if (above.HasTile && !current.HasTile)
                    return (startX, checkY);
            }
            return (-1, -1);
        }

        //Finds the first air block above a solid tile
        public (int x, int y) FindAirAboveSolid(int startX, int startY, int maxDown)
        {
            for (int offset = 0; offset <= maxDown; offset++)
            {
                int checkY = startY + offset;
                if (checkY >= Main.maxTilesY - 2)
                    break;
                Tile above = Framing.GetTileSafely(startX, checkY);
                Tile below = Framing.GetTileSafely(startX, checkY + 1);
                // if it finds a rail, turn that into a pressure plate rail instead
                if (above.HasTile && above.TileType == 314 && below.HasTile && Main.tileSolid[below.TileType])
                {
                    Terraria.WorldGen.KillTile(startX, checkY, noItem: true);
                    Terraria.WorldGen.PlaceTile(startX, checkY, 314, forced: true, style: 1);
                    return (startX, checkY);
                }
                if (!above.HasTile && below.HasTile && Main.tileSolid[below.TileType])
                    return (startX, checkY);
            }
            return (-1, -1);
        }

        //properly wires traps
        private void WireHorizontalAndVertical(int x1, int y1, int x2, int y2)
        {
            int stepX = (x2 > x1) ? 1 : -1;
            for (int x = x1; x != x2; x += stepX)
            {
                Framing.GetTileSafely(x, y1).RedWire = true;
                Terraria.WorldGen.SquareTileFrame(x, y1);
            }
            int stepY = (y2 > y1) ? 1 : -1;
            for (int y = y1; y != y2; y += stepY)
            {
                Framing.GetTileSafely(x2, y).RedWire = true;
                Terraria.WorldGen.SquareTileFrame(x2, y);
            }
            Framing.GetTileSafely(x2, y2).RedWire = true;
            Terraria.WorldGen.SquareTileFrame(x2, y2);
        }

        private (int x, int y) GetRandomCavernOrUndergroundSpot()
        {
            int cavernStart = (int)Main.rockLayer;
            int cavernEnd = Main.maxTilesY - 200;
            int x = Terraria.WorldGen.genRand.Next(50, Main.maxTilesX - 50);
            int y = Terraria.WorldGen.genRand.Next(cavernStart / 2, cavernEnd);
            return (x, y);
        }
    }
}

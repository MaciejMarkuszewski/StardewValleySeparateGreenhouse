using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using StardewModdingAPI.Events;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace SeparateGreenhouse
{
    class SeparateGreenhouseMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
           // Helper.Events.GameLoop.Saved += this.Saved;
            Helper.Events.GameLoop.SaveLoaded += this.Loaded;
            Helper.Events.Multiplayer.PeerContextReceived += this.Connected; ;
            Helper.Events.Player.Warped += this.Warped;
        }
        private void Connected(object sender, PeerContextReceivedEventArgs e)
        {
            if (Context.IsMainPlayer && !e.Peer.IsHost)
            {
                NetCollection<Building> buildings = new NetCollection<Building>();
                foreach (Building cabin in Game1.getFarm().buildings)
                {
                    if (cabin.buildingType.Value.Contains("Cabin"))
                    {
                        foreach(Building greenhouse in Game1.getFarm().buildings)
                        {
                            if(greenhouse.buildingType.Value.Contains("Greenhouse") && greenhouse.owner.Value == (cabin.indoors.Value as Cabin).getFarmhand().Value.UniqueMultiplayerID)
                            {
                                return;
                            }
                        }
                        Building Greenhouse = new Building(new BluePrint("Greenhouse"), new Vector2(-10000f, 0.0f));
                        Greenhouse.owner.Value = (cabin.indoors.Value as Cabin).getFarmhand().Value.UniqueMultiplayerID;
                        Greenhouse.indoors.Value.warps[0] = Game1.getLocationFromName("Greenhouse").warps[0];
                        buildings.Add(Greenhouse);
                    }
                }
                foreach(Building greenhouse in buildings)
                {
                    Game1.getFarm().buildings.Add(greenhouse);
                }
            }
        }

        private void Warped(object sender, WarpedEventArgs e)
        {
            if (!Context.IsMainPlayer && e.NewLocation.Name.Equals("Greenhouse") && e.OldLocation.Name.Equals("Farm"))
            {
                foreach(Building building in Game1.getFarm().buildings)
                {
                    if (building.buildingType.Value.Contains("Greenhouse") && building.owner.Value == Game1.player.UniqueMultiplayerID)
                    {
                        Game1.warpFarmer(building.nameOfIndoors, 10, 23, false);
                    }
                }
            }
        }

        private void Loaded(object sender, EventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                foreach(Building building in Game1.getFarm().buildings)
                {
                    if (building.buildingType.Value.Contains("Greenhouse"))
                    {
                        foreach(StardewValley.Object obj in building.indoors.Value.objects.Values)
                        {
                            obj.boundingBox.Value = new Rectangle((int)obj.TileLocation.X, (int)obj.TileLocation.Y, obj.boundingBox.Value.Width, obj.boundingBox.Value.Height);
                        }
                        building.indoors.Value.warps[0] = Game1.getLocationFromName("Greenhouse").warps[0];
                    }
                }
            }
        }
    }
}
using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Services
{
    public interface IProgramCore
    {
        public void Restart();

        public void StartDebugging();

        public void StopDebugging();

        public void Pause();

        public void Continue();

        public void Step();

        //public bool Login(string name, string password);

        //public IEnumerable<Room> GetRooms(string username);

        //public IEnumerable<Category> GetCategories(string username);

        //public IEnumerable<AVisualBlock> GetBlocksInRoom(Room room);

        //public IEnumerable<AVisualBlock> GetBlocksInCategory(Category category);

        public List<User> Users { get; }

        public List<Room> Rooms { get; }

        public List<Category> Categories { get; }

        public List<VisualBlock> VisualBlocks { get; }

        public double AverageCpuUsage { get; }
        public double MaxCpuUsage { get; }

    }
}
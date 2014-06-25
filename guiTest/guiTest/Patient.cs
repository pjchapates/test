using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace guiTest {
    class Patient { 
        public int sensorID;
        public int roomNumber;
        public string name;
        public Patient(int sensorID, int roomNumber, string name) {
            this.sensorID = sensorID;
            this.roomNumber = roomNumber;
            this.name = name;
        }
    }
}

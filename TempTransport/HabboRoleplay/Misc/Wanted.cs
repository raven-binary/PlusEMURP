using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboRoleplay.Misc;
using Plus.HabboRoleplay.RoleplayUsers;

namespace Plus.HabboRoleplay.Misc
{
   public class Wanted
    {
        /// <summary>
        /// The users roleplay instance
        /// </summary>
        RoleplayUser RoleplayUser;

        public bool IsWanted;
        public int WantedId = 0;
        public Dictionary<string, int> Charges = new Dictionary<string, int>();
        public double Date;
        public DateTime AddedTime;
        public bool Passed;
        public int Assault;
        public int Murder;
        public int Copassault;
        public int Copmurder;
        public int Ganghomicide;
        public int Obstruction;
        public int Hacking;
        public int Trespassing;
        public int Robbery;
        public int Illegalarea;
        public int Jailbreak;
        public int Terrorism;
        public int Drugs;
        public int Execution;
        public int Escaping;
        public int NonCompliance;
        public int CallAbuse;

        public Wanted(RoleplayUser RoleplayUser, DataRow WantedList)
        {
            this.RoleplayUser = RoleplayUser;
            this.Date = Convert.ToDouble(WantedList["date"]);
            this.AddedTime = Convert.ToDateTime(WantedList["added_date"]);
            this.Passed = PlusEnvironment.EnumToBool(WantedList["passed"].ToString());
            this.IsWanted = PlusEnvironment.EnumToBool(WantedList["wanted"].ToString());
            this.Assault = Convert.ToInt32(WantedList["assault"]);
            this.Murder = Convert.ToInt32(WantedList["murder"]);
            this.Copassault = Convert.ToInt32(WantedList["copassault"]);
            this.Copmurder = Convert.ToInt32(WantedList["copmurder"]);
            this.Ganghomicide = Convert.ToInt32(WantedList["ganghomicide"]);
            this.Obstruction = Convert.ToInt32(WantedList["obstruction"]);
            this.Hacking = Convert.ToInt32(WantedList["hacking"]);
            this.Trespassing = Convert.ToInt32(WantedList["trespassing"]);
            this.Robbery = Convert.ToInt32(WantedList["robbery"]);
            this.Illegalarea = Convert.ToInt32(WantedList["illegalarea"]);
            this.Jailbreak = Convert.ToInt32(WantedList["jailbreak"]);
            this.Terrorism = Convert.ToInt32(WantedList["terrorism"]);
            this.Drugs = Convert.ToInt32(WantedList["drugs"]);
            this.Execution = Convert.ToInt32(WantedList["execution"]);
            this.Escaping = Convert.ToInt32(WantedList["escaping"]);
            this.NonCompliance = Convert.ToInt32(WantedList["nonCompliance"]);
            this.CallAbuse = Convert.ToInt32(WantedList["callAbuse"]);
        }

        public void Load()
        {
            if (Assault > 0)
                Charges.Add("assault", 0);
            if (Murder > 0)
                Charges.Add("murder", 0);
            if (Copassault > 0)
                Charges.Add("copassault", 0);
            if (Copmurder > 0)
                Charges.Add("copmurder", 0);
            if (Ganghomicide > 0)
                Charges.Add("ganghomicide", 0);
            if (Obstruction > 0)
                Charges.Add("obstruction", 0);
            if (Hacking > 0)
                Charges.Add("hacking", 0);
            if (Trespassing > 0)
                Charges.Add("trespassing", 0);
            if (Robbery > 0)
                Charges.Add("robbery", 0);
            if (Illegalarea > 0)
                Charges.Add("illegalarea", 0);
            if (Jailbreak > 0)
                Charges.Add("jailbreak", 0);
            if (Terrorism > 0)
                Charges.Add("terrorism", 0);
            if (Drugs > 0)
                Charges.Add("drugs", 0);
            if (Execution > 0)
                Charges.Add("execution", 0);
            if (Escaping > 0)
                Charges.Add("escaping", 0);
            if (NonCompliance > 0)
                Charges.Add("non-compliance", 0);
            if (CallAbuse > 0)
                Charges.Add("911abuse", 0);

            if (IsWanted)
                Add();

            RoleplayManager.LoadWantedList();
        }

        public int ArrestTime()
        {
            int Minutes = 0;
            #region Get Minutes
            for (int i = 1; i <= Assault; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= Murder; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= Copassault; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= Copmurder; i++)
            {
                Minutes += 12;
            }
            for (int i = 1; i <= Ganghomicide; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= Obstruction; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= Hacking; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= Trespassing; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= Robbery; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= Illegalarea; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= Jailbreak; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= Terrorism; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= Drugs; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= Execution; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= Escaping; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= NonCompliance; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= CallAbuse; i++)
            {
                Minutes += 3;
            }

            if (Minutes > 45)
                Minutes = 45;
            #endregion
            return Minutes;
        }

        public int GetArrestTimeByCharge(string value)
        {
            #region Get Minutes
            if (value == "assault")
            {
                return Assault;
            }
            else if (value == "murder")
            {
                return Murder;
            }
            else if (value == "copassault")
            {
                return Copassault;
            }
            else if (value == "copmurder")
            {
                return Copmurder;
            }
            else if (value == "ganghomicide")
            {
                return Ganghomicide;
            }
            else if (value == "obstruction")
            {
                return Obstruction;
            }
            else if (value == "hacking")
            {
                return Hacking;
            }
            else if (value == "trespassing")
            {
                return Trespassing;
            }
            else if (value == "robbery")
            {
                return Robbery;
            }
            else if (value == "illegalarea")
            {
                return Illegalarea;
            }
            else if (value == "jailbreak")
            {
                return Jailbreak;
            }
            else if (value == "terrorism")
            {
                return Terrorism;
            }
            else if (value == "drugs")
            {
                return Drugs;
            }
            else if (value == "execution")
            {
                return Execution;
            }
            else if (value == "escaping")
            {
                return Escaping;
            }
            else if (value == "nonCompliance")
            {
                return NonCompliance;
            }
            else if (value == "callAbuse")
            {
                return CallAbuse;
            }
            else
                return 0;
            #endregion
        }

        public void Add()
        {
            if (!IsWanted)
            {
                IsWanted = true;
            }
            else
            {
                if (!RoleplayManager.WantedList.ContainsKey(RoleplayUser.Habbo.Id))
                {
                    RoleplayManager.WantedList.Add(RoleplayUser.Habbo.Id, RoleplayUser.Habbo.Username);
                    WantedId = RoleplayManager.WantedList.Count;
                    RoleplayUser.SendWeb("wanted;show");
                }
                RoleplayManager.LoadWantedList();
                return;
            }

            AddedTime = DateTime.Now;
            RoleplayManager.WantedList.Add(RoleplayUser.Habbo.Id, RoleplayUser.Habbo.Username);
            WantedId = RoleplayManager.WantedList.Count;
            RoleplayManager.LoadWantedList();
            RoleplayUser.SendWeb("wanted;show");
        }

        public void Remove()
        {
            if (IsWanted)
            {
                IsWanted = false;
                RoleplayUser.SendWeb("wanted;hide");
            }

            AddedTime = Convert.ToDateTime("2021-09-10 14:17:48");
            Passed = false;

            if (ArrestTime() < 15)
            {
                Assault = 0;
                Murder = 0;
                Copassault = 0;
                Copmurder = 0;
                Ganghomicide = 0;
                Obstruction = 0;
                Hacking = 0;
                Trespassing = 0;
                Robbery = 0;
                Illegalarea = 0;
                Jailbreak = 0;
                Terrorism = 0;
                Drugs = 0;
                Execution = 0;
                Escaping = 0;
                NonCompliance = 0;
                CallAbuse = 0;
                Charges.Clear();
                RoleplayManager.WantedList.Remove(RoleplayUser.Habbo.Id);
            }

            RoleplayManager.LoadWantedList(true, WantedId);
            WantedId = 0;
        }
    }
}

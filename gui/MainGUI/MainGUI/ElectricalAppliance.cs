using System.Threading;

namespace MainGUI
{
    public struct ElectricalApplianceData
    {
        public double? U;
        public double? I;
        public double? P;

        public ElectricalApplianceData(double u, double i)
        {
            U = u;
            I = i;
            P = null;
        }

        public ElectricalApplianceData(double p)
        {
            U = I = null;
            P = p;
        }
    }

    public class ElectricalAppliance
    {
        private CalculateCOPandP calculateCOPandP = new CalculateCOPandP();

        // number of Electrical Appliance instances (number of Electrical appliances)
        public static int NumberOfInstances = 0;

        public ElectricalAppliance()
        {
            Interlocked.Increment(ref NumberOfInstances);
        }

        ~ElectricalAppliance()
        {
            Interlocked.Decrement(ref NumberOfInstances);
        }

        /// <summary>
        /// Calculates the Electrical capacity of appliance
        /// </summary>
        /// <param name="U">Voltage [V]</param>
        /// <param name="I">Current [A]</param>
        /// <returns>Electrical Capacity [W]. <see cref="T:null" /> if DLL is not found.</returns>
        public double CalcAppliance(double U, double I)
        {
            return calculateCOPandP.CalculatePe(U, I) ?? 0;
        }
    }
}
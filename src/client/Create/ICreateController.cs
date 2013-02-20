namespace AsimovClient.Create
{
    public enum CreateMode
    {
        Passive,
        Safe,
        Full
    }

    public interface ICreateController
    {
        // Core
        void PowerOn();
        void PowerOff();
        void SetMode(CreateMode mode);

        // Drive
        void Drive(int velocity, int radius, int distance);
        void Turn(int velocity, int radius, int degrees);
        void Stop();

        // LED
        //TODO

        // Song
        //TODO
    }
}

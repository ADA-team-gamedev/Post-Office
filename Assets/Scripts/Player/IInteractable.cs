namespace Player
{
    public interface IInteractable
    {
        public void StartInteract(Interactor interactor);

        public void UpdateInteract(Interactor interactor);

        public void StopInteract(Interactor interactor);
    }
}

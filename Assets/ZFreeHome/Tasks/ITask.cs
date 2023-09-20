
public interface ITask
{
    public bool completed { get; set; }
    public bool active{ get; set; }
    public string taskDescription{ get; set; }
    
    public string GetDescription();
    void Complete();
    public void Activate();
    public void Deactivate();
}
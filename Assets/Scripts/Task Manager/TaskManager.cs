using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{   
    [SerializeField] private GameObject _taskTextPrefab;
    [SerializeField] private Transform _taskPanel;

    [Header("TasksPublishers")]
    [SerializeField] private PublisherSort _publisherSort;  


    [SerializeField] private List<TextMeshProUGUI> _curTaskTexts;
    public string[] Tasks;
    private void Start()
    {
        SetTask(Random.Range(0, Tasks.Length));
    }
    private void SetTask(int taskIndex)
    {
        ResetTasks();
        switch(taskIndex)
        {
            case 0:
                _publisherSort._sortEvent.AddListener(OnEvent);
                break;
        }
        string task = Tasks[taskIndex];
        if(_curTaskTexts != null)
        {
           _curTaskTexts.Clear();
        }
        foreach(Transform prevTask in _taskPanel)
        {
            Destroy(prevTask.gameObject);
        }
        string[] curtask = task.Split('|');
        foreach(string curLine in curtask)
        {
            TextMeshProUGUI text = Instantiate(_taskTextPrefab, _taskPanel).GetComponent<TextMeshProUGUI>();
            text.text = curLine;
            _curTaskTexts.Add(text);           
        }
    }
    private void ResetTasks()
    {
        _publisherSort._sortEvent.RemoveListener(OnEvent);
    }
    private void OnEvent(int lineIndex)
    {
        Destroy(_curTaskTexts[lineIndex].gameObject);
        if (_taskPanel.childCount <= 0)
        {
            TaskComplete();
        }
    }
    private void TaskComplete()
    {
        Debug.Log("task completed :)");
        SetTask(Random.Range(0, Tasks.Length));
    }  
}

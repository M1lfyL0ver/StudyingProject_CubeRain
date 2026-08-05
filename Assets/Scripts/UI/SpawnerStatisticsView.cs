using TMPro;
using UnityEngine;

public sealed class SpawnerStatisticsView : MonoBehaviour
{
    [SerializeField] private SpawnerStatisticsSource _statisticsSource;
    [SerializeField] private TextMeshProUGUI _spawnedObjectsText;
    [SerializeField] private TextMeshProUGUI _createdObjectsText;
    [SerializeField] private TextMeshProUGUI _activeObjectsText;

    private void OnEnable()
    {
        if (_statisticsSource == null)
            return;

        _statisticsSource.StatisticsChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (_statisticsSource != null)
            _statisticsSource.StatisticsChanged -= Refresh;
    }

    private void Refresh()
    {
        _spawnedObjectsText.text = _statisticsSource.SpawnedObjectsCount.ToString();
        _createdObjectsText.text = _statisticsSource.CreatedObjectsCount.ToString();
        _activeObjectsText.text = _statisticsSource.ActiveObjectsCount.ToString();
    }
}
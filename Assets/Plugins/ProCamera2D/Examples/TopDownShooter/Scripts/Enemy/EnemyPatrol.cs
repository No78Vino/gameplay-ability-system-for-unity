using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class EnemyPatrol : MonoBehaviour
    {
        public Transform PathContainer;
        public float WaypointOffset = .1f;
        public bool Loop = true;
        public bool IsPaused;

        NavMeshAgent _navMeshAgent;
        List<Transform> _path;
        int _currentWaypoint;
        bool _hasReachedDestination;
        float _stopTime;

        void Awake ()
        {
            _navMeshAgent = this.GetComponentInChildren<NavMeshAgent>();

            _path = new List<Transform>();
            if(PathContainer != null)
            {
                foreach (Transform child in PathContainer)
                {
                    _path.Add(child);
                }
            }
            else
            {
                Debug.LogWarning("No path set.");
            }
        }

        void Update()
        {
            if(IsPaused)
                return;

            if (_navMeshAgent.remainingDistance <= WaypointOffset && !_hasReachedDestination)
            {
                _hasReachedDestination = true;

                PatrolWaypoint patrolWaypoint = _path[_currentWaypoint].GetComponent<PatrolWaypoint>();
                if (patrolWaypoint != null)
                {
                    _stopTime = Random.Range(patrolWaypoint.StopDuration - patrolWaypoint.StopDurationVariation, patrolWaypoint.StopDuration + patrolWaypoint.StopDurationVariation);
                    if (Random.value >= patrolWaypoint.StopProbability)
                    {
                        GoToNextWaypoint();
                    }
                }
                else
                {
                    GoToNextWaypoint();
                }
            }

            if (_hasReachedDestination)
            {
                _stopTime -= Time.deltaTime;

                if (_stopTime <= 0)
                    GoToNextWaypoint();
            }
        }

        public void StartPatrol()
        {
            GoToWaypoint(0);
        }

        public void PausePatrol()
        {
            IsPaused = true;

            _navMeshAgent.isStopped = true;
        }

        public void ResumePatrol()
        {
            GoToWaypoint(_currentWaypoint);
        }

        void GoToNextWaypoint()
        {
            if (_currentWaypoint < _path.Count - 1)
            {
                _currentWaypoint++;
            }
            else
            {
                if (Loop)
                {
                    _currentWaypoint = 0;
                }
                else
                {
                    Debug.Log("Path completed");
                }
            }
            GoToWaypoint(_currentWaypoint);
        }

        void GoToWaypoint(int waypoint)
        {
            IsPaused = false;

            _hasReachedDestination = false;
            _currentWaypoint = waypoint;

            Vector3 destination = new Vector3(_path[_currentWaypoint].position.x, _navMeshAgent.transform.position.y, _path[_currentWaypoint].position.z);
            _navMeshAgent.SetDestination(destination);
        }
    }
}
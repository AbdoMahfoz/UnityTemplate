using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public string[] Events;
    [HideInInspector] public Dictionary<string, List<Action<object>>> Subscribers;

    public void ListenToEvent<T>(string SubscriberName, string Event, Action<T> Callback)
    {
        if (!Events.Contains(Event))
            throw new ArgumentException(
                $"{SubscriberName} attempted to subscribe to event {Event}, which is not listed in the event list");
        Subscribers[Event].Add(o =>
        {
            T val;
            try
            {
                val = (T) o;
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Incompatible event types for event {Event};\n" +
                                                    $"The source fired the event with an argument of type {o.GetType().Name}\n" +
                                                    $"{SubscriberName} expected an argument of type {typeof(T).Name}\n");
            }

            Callback(val);
        });
    }

    public void FireEvent<T>(string SourceName, string Event, T arg)
    {
        if (!Events.Contains(Event))
            throw new ArgumentException(
                $"{SourceName} attempted to fire event {Event}, which is not listed in the event list");
        Debug.Log($"{SourceName} fired {Event}");
        foreach (var callback in Subscribers[Event])
        {
            callback(arg);
        }
    }
}
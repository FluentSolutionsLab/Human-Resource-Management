﻿namespace HRManagement.Common.Application.Contracts;

public interface ICacheService
{
    T Get<T>(string key);
    void Set<T>(string key, T value);
    void Remove(string key);
    void RemoveAll(Func<string, bool> condition);
}
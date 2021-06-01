using System;

public interface IShadowOwner
{
    event Action<bool> OnChangeGroundedState;
}
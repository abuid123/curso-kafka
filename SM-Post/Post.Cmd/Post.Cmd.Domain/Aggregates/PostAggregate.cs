using System;
using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
  private bool _active;
  private string _author;
  private readonly Dictionary<Guid,Tuple<string,string>> _comments = new();

  public bool Active  
  {
    get => _active;  set => _active = value;
  }

  public PostAggregate()
  {
    
  }
  public PostAggregate(Guid id, string author, string message)
  {
    RaiseEvent(new PostCreatedEvent
    {
      id = id,
      Author = author,
      Message = message,
      DatePosted = DateTime.Now
    });
  }

  public void Apply(PostCreatedEvent @event)
  {
    _active = true;
    _author = @event.Author;
    _id = @event.id;
  }

  public void EditMessage(string message)
  {
    if (!_active)
    {
      throw new InvalidOperationException("You cannot edit message of an inactive post!");
    }

    if (string.IsNullOrWhiteSpace(message)){
      throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty.");
    }

    RaiseEvent(new MessageUpdatedEvent
    {
      id = _id,
      Message = message
    });
  }

  public void Apply(MessageUpdatedEvent @event)
  {
    _id = @event.id;
  }

  public void LikePost()
  {
    if (!_active)
    {
      throw new InvalidOperationException("You cannot like an inactive post!");
    }

    RaiseEvent(new PostLikedEvent
    {
      id = _id,
    });
  }

  public void Apply(PostLikedEvent @event)
  {
    _id = @event.id;
  }

  public void AddComment(string comment, string username)
  {
    if (!_active)
    {
      throw new InvalidOperationException("You cannot add a comment to an inactive post!");
    }

    if (string.IsNullOrWhiteSpace(comment)){
      throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty.");
    }

    RaiseEvent(new CommentAddedEvent
    {
      id = _id,
      CommentId = Guid.NewGuid(),
      Comment = comment,
      Username = username,
      Date = DateTime.Now
    });
  }

  public void Apply(CommentAddedEvent @event)
  {
    _id = @event.id;
    _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
  }
}

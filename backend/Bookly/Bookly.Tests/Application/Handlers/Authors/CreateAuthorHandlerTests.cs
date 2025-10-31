using Bookly.Application.Handlers.Authors;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Author;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Authors;

[TestFixture]
public class CreateAuthorHandlerTests
{
    private BooklyDbContext _db = null!;
    private IMediator _mediator = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _mediator = Substitute.For<IMediator>();
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_CreatesNewAuthor_WhenNotExists()
    {
        _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
            .Returns((Author?)null);

        var handler = new CreateAuthorHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
        var dto = new CreateAuthorDto("Александр Пушкин", "Александр Пушкин");
        var command = new CreateAuthorCommand(dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value.Name, Is.EqualTo("Пушкин А."));
        Assert.That(await _db.Authors.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_ReturnsExisting_WhenFoundByStandardName()
    {
        var existing = Author.Create(new CreateAuthorDto("Пушкин А.С.", "Александр Сергеевич Пушкин")).Value;
        _db.Authors.Add(existing);
        await _db.SaveChangesAsync();

        _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var query = callInfo.Arg<GetAuthorQuery>();
                return query.Name == "Александр Сергеевич Пушкин" ? existing : null;
            });

        var handler = new CreateAuthorHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
        var dto = new CreateAuthorDto("Александр Сергеевич Пушкин", "display");
        var command = new CreateAuthorCommand(dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value.Id, Is.EqualTo(existing.Id));
        Assert.That(await _db.Authors.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_CreatesNewAuthor_WhenTwoLettersName()
    {
        _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
            .Returns((Author?)null);

        var handler = new CreateAuthorHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
        var dto = new CreateAuthorDto("Александр Пушкин", "Александр Пушкин");
        var command = new CreateAuthorCommand(dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value.Name, Is.EqualTo("Пушкин А."));
        Assert.That(await _db.Authors.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_CreatesNewAuthor_WhenFullNameIsAlreadyStandardForm()
    {
        _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
            .Returns((Author?)null);

        var handler = new CreateAuthorHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
        var dto = new CreateAuthorDto("Пушкин А.С.", "Пушкин А.С.");
        var command = new CreateAuthorCommand(dto);

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess);
        Assert.That(result.Value.Name, Is.EqualTo("Пушкин А.С."));
        Assert.That(await _db.Authors.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_CreatesNewAuthor_WhenFullNameContainsThreeWords()
    {
        _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
            .Returns((Author?)null);

        var handler = new CreateAuthorHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
        var dto = new CreateAuthorDto("Александр Сергеевич Пушкин", "Александр Сергеевич Пушкин");
        var command = new CreateAuthorCommand(dto);

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess);
        Assert.That(result.Value.Name, Is.EqualTo("Пушкин А.С."));
        Assert.That(result.Value.DisplayName, Is.EqualTo("Александр Сергеевич Пушкин"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenFactoryFails()
    {
        _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
            .Returns((Author?)null);

        var handler = new CreateAuthorHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
        var dto = new CreateAuthorDto("", "");
        var command = new CreateAuthorCommand(dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(await _db.Authors.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task Handle_ReturnsExisting_WhenFoundByBestFormat()
    {
        var existing = Author.Create(new CreateAuthorDto("Пушкин А.", "Александр Пушкин")).Value;
        _db.Authors.Add(existing);
        await _db.SaveChangesAsync();

        _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var query = callInfo.Arg<GetAuthorQuery>();
                return query.Name == "Пушкин А." ? existing : null;
            });

        var handler = new CreateAuthorHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
        var dto = new CreateAuthorDto("Александр Пушкин", "Александр Пушкин");
        var command = new CreateAuthorCommand(dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value.Id, Is.EqualTo(existing.Id));
        Assert.That(await _db.Authors.CountAsync(), Is.EqualTo(1));
    }
}
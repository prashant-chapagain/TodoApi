using Microsoft.EntityFrameworkCore;
using TodoApi.Project;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList "));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app=builder.Build();

var todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/todoitems", async (TodoDb db) =>
await db.Todos.ToListAsync());

todoItems.MapGet("/todoitems/complete",async (TodoDb db)=>
await db.Todos.Where(t=>t.IsComplete).ToListAsync());

todoItems.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
await db.Todos.FindAsync(id)
    is Todo todo
    ? Results.Ok(todo)
    : Results.NotFound());
todoItems.MapPost("/todoitems", async (Todo todo,TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
        
}

);
todoItems.MapPut("/todoitems/{id}", async (int id,Todo updatedTodo, TodoDb db) => {
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Name = updatedTodo.Name;
    todo.IsComplete = updatedTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();

    }
);
todoItems.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if(await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound() ;
});

app.Run();

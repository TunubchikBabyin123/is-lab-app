
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Данные в памяти
var notes = new List<Note>();
int nextId = 1;

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));

// Version
var appName = builder.Configuration["App:Name"] ?? "IsLabApp";
var appVersion = builder.Configuration["App:Version"] ?? "0.1.0";
app.MapGet("/version", () => Results.Ok(new { name = appName, version = appVersion }));

// GET /api/notes
app.MapGet("/api/notes", () => Results.Ok(notes));

// GET /api/notes/{id}
app.MapGet("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    return note is null ? Results.NotFound() : Results.Ok(note);
});

// POST /api/notes
app.MapPost("/api/notes", (CreateNoteRequest request) =>
{
    var note = new Note
    {
        Id = nextId++,
        Title = request.Title,
        Text = request.Text,
        CreatedAt = DateTime.UtcNow
    };
    notes.Add(note);
    return Results.Created($"/api/notes/{note.Id}", note);
});

// DELETE /api/notes/{id}
app.MapDelete("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    if (note is null) return Results.NotFound();
    notes.Remove(note);
    return Results.NoContent();
});

// DB ping (заглушка)
app.MapGet("/db/ping", () => Results.Ok(new { connected = false, message = "Database not configured yet (will be added in Lab7)" }));

app.Run();

record Note { public int Id { get; set; } public string Title { get; set; } = ""; public string Text { get; set; } = ""; public DateTime CreatedAt { get; set; } }
record CreateNoteRequest(string Title, string Text);
using System;
using Microsoft.EntityFrameworkCore;
using Person.Data;
using Person.Models;

namespace Person.Routes;

public static class PersonRoute
{
    public static void PersonRoutes(this WebApplication app)
    {
        var route = app.MapGroup("person");
        route.MapPost("", async (PersonRequest req, PersonContext context) =>
        {
            var person = new PersonModel(req.name);
            await context.AddAsync(person);
            await context.SaveChangesAsync();
            return Results.Created($"/person/{person.Id}", person);
        });

        route.MapGet("", async (PersonContext context) =>
        {
            var people = await context.People.ToListAsync();

            if (people is null || people.Count == 0)
            {
                return Results.NotFound();
            }

            return Results.Ok(people);

        });

        route.MapPut("{id:guid}", async (Guid id, PersonRequest req, PersonContext context) =>
        {
            var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);

            //Outra forma de fazer a mesma coisa:
            // var person = await context.People.FindAsync(id);

            if (person is null)
            {
                return Results.NotFound();
            }

            person.ChangeName(req.name);
            await context.SaveChangesAsync();

            return Results.Ok(person);
        });
        //soft delete
        route.MapDelete("{id:guid}", async (Guid id, PersonContext context) =>
        {
            var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);

              if (person is null)
            {
                return Results.NotFound();
            }

            person.SetInactive();
            await context.SaveChangesAsync();
            return Results.Ok(person);

        });

        //hard delete
        route.MapDelete("delete/{id:guid}", async (Guid id, PersonContext context) =>
        {
            var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);

              if (person is null)
            {
                return Results.NotFound();
            }

            context.Remove(person);
            await context.SaveChangesAsync();
            return Results.Ok(person);

        });
    }
} 

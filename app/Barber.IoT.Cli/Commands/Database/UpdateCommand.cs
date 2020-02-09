namespace Barber.IoT.Cli.Commands.Database
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Barber.Cli.Helper;
    using Barber.IoT.Context;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.EntityFrameworkCore;

    public class UpdateCommand
    {
        public static void Register(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Check and update database";
            config.ExtendedHelpText = "Check database and make updates if needed";

            config.OnExecuteAsync(
                async cancellationToken =>
                {
                    try
                    {
                        Styler.DivisionLine('-');
                        Styler.AlignCenter("Bootstrap");
                        Styler.DivisionLine('-');

                        var contextCreator = Program.Container.GetInstance<IBarberIoTContextCreator>();

                        Console.WriteLine(@"We will connect to the following database");
                        using (var context = contextCreator.CreateDbContext())
                        {
                            Console.WriteLine(context.Database.GetDbConnection().ConnectionString);
                            if (!Styler.ReadInputAsBool("Is that correct?"))
                            {
                                return 1;
                            }

                            Console.Write(@"Try to connect to server...");
                            if (!await context.Database.CanConnectAsync(cancellationToken))
                            {
                                Styler.Error("Could not connect to server or find database");

                                if (!Styler.ReadInputAsBool("We could not find database should we create it?"))
                                {
                                    return 1;
                                }

                                await context.Database.MigrateAsync(cancellationToken);

                                return 0;
                            }

                            Styler.Success("ok");

                            var migrationApplied = (await context.Database.GetAppliedMigrationsAsync(cancellationToken: cancellationToken)).ToArray();
                            var migrationAvailable = context.Database.GetMigrations().ToArray();

                            Console.Write(@"Read migrations...");
                            if (migrationApplied.Length != migrationAvailable.Length)
                            {
                                Styler.Error("Version miss match" + Environment.NewLine);
                                Console.WriteLine($@"Applied Migrations: {migrationApplied.Length} [#]");
                                Console.WriteLine($@"Available Migrations: {migrationAvailable.Length} [#]" + Environment.NewLine);
                                Console.WriteLine($@"Applied Migration: {migrationApplied.LastOrDefault()} [#]");
                                Console.WriteLine($@"Latest Migration: {migrationAvailable.LastOrDefault()} [#]");

                                if (!Styler.ReadInputAsBool("We have missing migration, should we apply them now?"))
                                {
                                    return 1;
                                }

                                Console.Write(@"Apply migrations...");

                                await context.Database.MigrateAsync(cancellationToken);
                            }
                        }

                        Styler.Success("ok");

                        await Task.FromResult(0);
                    }
                    catch (Exception exception)
                    {
                        Styler.Error(exception);

                        return 1;
                    }

                    return 0;
                });
        }
    }
}

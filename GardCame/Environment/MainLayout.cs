using Spectre.Console;

namespace GardCame.Environment
{
    public class MainLayout : MainBase
    {
        public static Layout mainLayout;

        public override void OnStart()
        {
            mainLayout = new Layout("Root").SplitRows
               (
                   new Layout("Players").Ratio(2),
                   new Layout("Mid").Ratio(3).SplitColumns
                       (
                           new Layout("Board").Ratio(2).SplitColumns
                           (
                               new Layout("LastCard"),
                               new Layout("Ammount").Ratio(2)
                           ),
                           new Layout("Menu").SplitRows
                           (
                               new Layout("Life"),
                               new Layout("Description").Ratio(2)
                           )
                       ),
                   new Layout("Deck").Size(11)
               );

            mainLayout["Menu"].Update(
                new Panel(" ").Expand()
                );

            mainLayout["Players"].Update(
                new Panel(" ").Expand()
                );

            mainLayout["LastCard"].Update(
               new Panel(" ").Expand().NoBorder()
               );

            mainLayout["Ammount"].Update(
              new Panel(" ").Expand().NoBorder()
              );

            mainLayout["Life"].Update(
              new Panel(" ").Expand().NoBorder()
              );
        }

        public override void OnUpdate()
        {
            AnsiConsole.Write(mainLayout);
        }
    }
}

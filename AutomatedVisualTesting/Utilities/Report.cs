using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedVisualTesting.Utilities
{
    public static class Report
    {
        public static void writeResults()
        {
            try
            {
                String reportIn = new String(Files.readAllBytes(Paths.get(templatePath)));
                for (int i = 0; i < details.size(); i++)
                {
                    reportIn = reportIn.replaceFirst(resultPlaceholder, "<tr><td>" + Integer.toString(i + 1) + "</td><td>" + details.get(i).getResult() + "</td><td>" + details.get(i).getResultText() + "</td></tr>" + resultPlaceholder);
                }

                String currentDate = new SimpleDateFormat("dd-MM-yyyy").format(new Date());
                String reportPath = "Z:\\Documents\\Bas\\blog\\files\\report_" + currentDate + ".html";
                Files.write(Paths.get(reportPath), reportIn.getBytes(), StandardOpenOption.CREATE);

            }
            catch (Exception e)
            {
                System.out.println("Error when writing report file:\n" + e.toString());
            }
        }
    }
}

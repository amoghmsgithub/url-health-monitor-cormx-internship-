using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Services
{
    public class ReportPdfService
    {
        public byte[] GeneratePdf(List<UrlOutage> outages)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.SetMargins(30, 30, 30, 30);

            // ================= HEADER =================
            var headerTable = new Table(1).UseAllAvailableWidth();

            headerTable.AddCell(
                new Cell()
                    .Add(new Paragraph("CORMX - Outage Summary Report")
                        .SetFontSize(18)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetTextAlignment(TextAlignment.CENTER))
                    .SetBackgroundColor(new DeviceRgb(33, 37, 41))
                    .SetPadding(12)
                    .SetBorder(Border.NO_BORDER)
            );

            document.Add(headerTable);

            document.Add(new Paragraph($"Generated on {DateTime.Now:dd MMM yyyy HH:mm} (Asia/Kolkata)")
                .SetFontSize(9)
                .SetFontColor(ColorConstants.GRAY)
                .SetTextAlignment(TextAlignment.RIGHT));

            document.Add(new Paragraph("\n"));

            var totalOutages = outages.Count;

            var totalDowntimeMinutes = outages
                .Where(o => o.RecoveredAt.HasValue)
                .Sum(o => (o.RecoveredAt.Value - o.DownStartedAt).TotalMinutes);

            // ================= EXECUTIVE SUMMARY =================
            document.Add(new Paragraph("Executive Summary")
                .SetFontSize(14));

            document.Add(new Paragraph(
                "This report provides an overview of monitored URL performance, outage events, and automated recovery actions during the reporting period."
            ).SetFontSize(10));

            document.Add(new Paragraph("\n"));

            // ================= SUMMARY CARDS =================
            var summaryTable = new Table(2).UseAllAvailableWidth();

            summaryTable.AddCell(CreateMetricCell("Total Outages", totalOutages.ToString()));
            summaryTable.AddCell(CreateMetricCell("Total Downtime", $"{Math.Round(totalDowntimeMinutes)} mins"));

            document.Add(summaryTable);

            document.Add(new Paragraph("\n"));

            // ================= PERFORMANCE INSIGHTS =================
            document.Add(new Paragraph("Performance Insights")
                .SetFontSize(14));

            document.Add(new Paragraph(
                $"A total of {totalOutages} outage events were recorded, resulting in {Math.Round(totalDowntimeMinutes)} minutes of downtime. " +
                "Availability percentages indicate overall reliability and recovery effectiveness."
            ).SetFontSize(10));

            document.Add(new Paragraph("\n"));

            // ================= TABLE =================
            document.Add(new Paragraph("Detailed Monitor Breakdown")
                .SetFontSize(14));

            document.Add(new Paragraph("\n"));

            var table = new Table(4).UseAllAvailableWidth();

            AddHeaderCell(table, "Monitor Name");
            AddHeaderCell(table, "Availability");
            AddHeaderCell(table, "Downtime");
            AddHeaderCell(table, "Status");

            foreach (var group in outages.GroupBy(o => o.MonitoredUrl))
            {
                var urlName = group.Key?.Name ?? "-";

                var downtime = group
                    .Where(o => o.RecoveredAt.HasValue)
                    .Sum(o => (o.RecoveredAt.Value - o.DownStartedAt).TotalMinutes);

                var totalMonitoringMinutes = 24 * 60;
                var availability = 100 - ((downtime / totalMonitoringMinutes) * 100);
                availability = Math.Max(0, availability);

                var status = downtime > 0 ? "Recovered" : "Healthy";

                table.AddCell(CreateDataCell(urlName));
                table.AddCell(CreateDataCell($"{availability:F2}%"));
                table.AddCell(CreateDataCell($"{Math.Round(downtime)} mins"));
                table.AddCell(CreateStatusCell(status));
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));

            // ================= STATUS DEFINITIONS =================
            document.Add(new Paragraph("Status Definitions")
                .SetFontSize(12));

            document.Add(new Paragraph("Healthy: No downtime recorded during reporting period.")
                .SetFontSize(9));

            document.Add(new Paragraph("Recovered: Outage occurred but was automatically restored.")
                .SetFontSize(9));

            document.Add(new Paragraph("\n"));

            // ================= FOOTER =================
            document.Add(new Paragraph("© 2026 Cormsquare - Generated by CORMX")
                .SetFontSize(8)
                .SetFontColor(ColorConstants.GRAY)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Close();
            return stream.ToArray();
        }

        private Cell CreateMetricCell(string title, string value)
        {
            return new Cell()
                .Add(new Paragraph(title)
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.GRAY))
                .Add(new Paragraph(value)
                    .SetFontSize(16))
                .SetPadding(12)
                .SetBackgroundColor(new DeviceRgb(245, 247, 250))
                .SetBorder(new SolidBorder(new DeviceRgb(220, 220, 220), 1));
        }

        private void AddHeaderCell(Table table, string text)
        {
            table.AddHeaderCell(new Cell()
                .Add(new Paragraph(text).SetFontSize(11))
                .SetBackgroundColor(new DeviceRgb(230, 230, 230))
                .SetPadding(8));
        }

        private Cell CreateDataCell(string text)
        {
            return new Cell()
                .Add(new Paragraph(text).SetFontSize(10))
                .SetPadding(6);
        }

        private Cell CreateStatusCell(string status)
        {
            var paragraph = new Paragraph(status).SetFontSize(10);

            if (status == "Healthy")
                paragraph.SetFontColor(new DeviceRgb(40, 167, 69));
            else
                paragraph.SetFontColor(new DeviceRgb(220, 53, 69));

            return new Cell()
                .Add(paragraph)
                .SetPadding(6);
        }
    }
}
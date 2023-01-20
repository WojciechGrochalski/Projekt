import {
  Component,
  OnInit,
  ChangeDetectorRef,
  ChangeDetectionStrategy,
} from "@angular/core";
import * as apex from "ng-apexcharts";
import { CashService } from "../../Services/cash.service";
import { Cash } from "../../Models/Cash";
import { UserService } from "../../Services/User.service";

@Component({
  selector: "app-cash-data",
  templateUrl: "./cash-data.component.html",
  styleUrls: ["./cash-data.component.css"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CashDataComponent implements OnInit {
  dropdownList = [];
  selectedItems = [];
  dropdownSettings = {};

  series: apex.ApexAxisChartSeries;
  title: apex.ApexTitleSubtitle;
  chart: apex.ApexChart;
  yaxis: apex.ApexYAxis;
  xaxis: apex.ApexXAxis;

  public cash_list: Cash[];
  public result: Cash[];

  public chartList = [];

  public commonOptions = {
    dataLabels: {
      enabled: false,
    },
    stroke: {
      curve: "straight",
    },
    toolbar: {
      tools: {
        selection: false,
      },
    },
    markers: {
      size: 6,
      hover: {
        size: 10,
      },
    },
    tooltip: {
      followCursor: false,
      theme: "dark",
      x: {
        show: false,
      },
      marker: {
        show: false,
      },
      y: {
        title: {
          formatter: function () {
            return "";
          },
        },
      },
    },
    grid: {
      clipMarkers: false,
    },
    xaxis: {
      type: "datetime",
    },
  };

  constructor(
    private cashService: CashService,
    private userService: UserService,
    private cd: ChangeDetectorRef
  ) {
    this.title = {
      text: "Waluty",
    };

    this.series = [
      {
        name: "Moja Waluta",
        data: [],
      },
    ];

    this.chart = {
      type: "line",
      toolbar: {
        show: false,
      },
    };
  }

  public generateDayWiseTimeSeries(baseval, count, yrange): any[] {
    let i = 0;
    let series = [];
    while (i < count) {
      var x = baseval;
      var y =
        Math.floor(Math.random() * (yrange.max - yrange.min + 1)) + yrange.min;

      series.push([x, y]);
      baseval += 86400000;
      i++;
    }
    return series;
  }

  async ngOnInit(): Promise<void> {
    this.dropdownList = [
      { id: "USD", text: "USD $" },
      { id: "EUR", text: "EUR €" },
      { id: "GBP", text: "Funt szterling £" },
      { id: "AUD", text: "Dolar australijski $" },
      { id: "CAD", text: "Dolar kanadyjski $" },
      { id: "HUF", text: "Forint (Węgry) Ft" },
      { id: "CZK", text: " Korona czeska Kč" },
      { id: "CHF", text: "Frank szwajcarski fr." },
      { id: "JPY", text: "Jen (Japonia) ¥" },
      { id: "DKK", text: "Korona duńska" },
      { id: "NOK", text: "Korona norweska" },
      { id: "SEK", text: "Korona Szwedzka" },
      { id: "XDR", text: "SDR" },
    ];
    this.dropdownSettings = {
      singleSelection: false,
      idField: "id",
      textField: "text",
      selectAllText: "Wybierz wszsytko",
      unSelectAllText: "Usuń zazanaczenie",
      itemsShowLimit: 3,
      enableCheckAll: false,
    };

    this.userService.SendBaseUrl().subscribe();
    try {
      this.cash_list = await this.cashService.GetDataOnInit().toPromise();
      this.cd.detectChanges();
    } catch (e) {
      console.error(e);
    }
    return;
  }

  onItemSelect(item: any) {
    this.TakeLastCurrency(item.id, "30");
  }

  onItemDeSelect(item: any) {
    this.chartList = this.chartList.filter((i) => {
      console.log(i)
      console.log(item)
      return i.chart.id !== item.id;
    });
    this.cd.detectChanges();
  }

  async TakeLastCurrency(iso: string, count: string) {
    let amountOfCurrency: number = +count;

    try {
      Promise.all([
        this.cashService.GetChartAskPrice(iso, amountOfCurrency).toPromise(),
        this.cashService.GetChartBidPrice(iso, amountOfCurrency).toPromise(),
        this.cashService.GetChartData(iso, amountOfCurrency).toPromise(),
      ]).then((values) => {
        this.CreateSomeShit(iso, values[0], values[1], values[2]);
      });
    } catch (e) {
      console.error(e);
    }
  }

  CreateSomeShit(name, buyValue, sellValue, data) {
    this.chartList = [
      ...this.chartList,
      {
        series: [
          {
            name: "Cena kupna",
            data: buyValue,
          },
          {
            name: "Cena sprzedaży",
            data: sellValue,
          },
        ],
        chart: {
          id: name,
          group: "social",
          type: "line",
          height: 300,
        },
        xaxis: {
          categories: data,
        },
        yaxis: {
          title: {
            text: "PLN",
          },
          min:
            Math.min.apply(null, sellValue) -
            Math.min.apply(null, sellValue) / 100,
          max:
            Math.max.apply(null, buyValue) +
            Math.max.apply(null, buyValue) / 100,
        },
        title: {
          text: name,
          align: "left",
        },
      },
    ];

    this.cd.detectChanges();
  }
}

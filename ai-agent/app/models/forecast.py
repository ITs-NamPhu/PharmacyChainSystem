from typing import Dict, List, Optional

from pydantic import BaseModel, Field


class SalesSummary(BaseModel):
    """Khối tổng hợp do backend .NET tính sẵn (SQL), LLM không cần tự toán."""

    totalQuantitySold: float = Field(0.0, description="Tổng số lượng bán trong kỳ")
    averageDaily: float = Field(0.0, description="Bình quân bán mỗi ngày")
    salesTrend: str = Field(
        "STABLE", description="INCREASING | DECREASING | STABLE (backend đánh giá sẵn)"
    )


class SalesHistoryResponse(BaseModel):
    productId: int
    productName: Optional[str] = None
    branchId: Optional[int] = None
    periodDays: int = 30
    unitName: Optional[str] = None
    summary: SalesSummary
    dailyRecords: Dict[str, float] = Field(
        default_factory=dict, description="Ngày (yyyy-MM-dd) -> số lượng bán, đã điền 0 cho ngày không bán"
    )


class ForecastDay(BaseModel):
    date: str
    tmin: Optional[float] = None
    tmax: Optional[float] = None
    humidity: Optional[float] = None
    rainMm: Optional[float] = None
    condition: Optional[str] = None


class WeatherForecast(BaseModel):
    location: str
    unit: str = "celsius"
    summary: str = ""
    daily: List[ForecastDay] = Field(default_factory=list)


class AirQualityDay(BaseModel):
    date: str
    aqi: Optional[int] = None
    pm2_5: Optional[float] = None
    pm10: Optional[float] = None
    level: Optional[str] = None


class AirQualityForecast(BaseModel):
    location: str
    summary: str = ""
    daily: List[AirQualityDay] = Field(default_factory=list)


class TrendKeyword(BaseModel):
    keyword: str
    avgInterest: float = 0.0
    slope: str = "flat"  # rising | falling | flat
    timeline: List[int] = Field(default_factory=list)


class SearchTrends(BaseModel):
    location: str
    days: int = 7
    keywords: List[TrendKeyword] = Field(default_factory=list)


class PublicHoliday(BaseModel):
    date: str
    name: str
    localName: Optional[str] = None
    daysUntil: Optional[int] = None

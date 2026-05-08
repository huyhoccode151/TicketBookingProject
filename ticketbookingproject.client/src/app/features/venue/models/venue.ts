export interface VenueListItemResponse {
  id: number;
  name: string;
  province: string;
  addressDetail: string;
  capacity: number;
  sectionCount: number;
}

export interface VenueDetailResponse {
  id: number;
  name: string;
  province: string;
  addressDetail: string;
  latitude?: number;
  longitude?: number;
  capacity: number;
  sections: VenueSectionResponse[];
  createdAt: string;
  updatedAt: string;
}

export interface VenueSectionResponse {
  id: number;
  name: string;
  capacity: number;
  seatCount: number;
}

import {Remainder} from "./Remainder";

export class CreatedUser {

  ID: number;
  Name: string;
  Email: string;
  Pass: string;
  Role:string;
  VerificationToken: string;
  IsVerify: boolean;
  Created: any;
  Subscriptions: boolean;
  Remainder: Remainder[];

}

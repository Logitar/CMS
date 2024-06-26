import { stringUtils } from "logitar-js";

const { isLetterOrDigit } = stringUtils;

export default function (s?: string): boolean {
  return typeof s === "string" && [...s].some((c) => !isLetterOrDigit(c));
}

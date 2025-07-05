import { saveAs } from "file-saver";
import { adminFileAPI } from "./adminFileAPI";

export async function exportToysToExcel() {
  try {
    const response = await adminFileAPI.get(
      "/ToysPriceUpdate/ExportToysToExcel"
    );
    const blob = response.data;
    saveAs(
      blob,
      `Toys_${new Date().toISOString().slice(0, 19).replace(/:/g, "")}.xlsx`
    );
  } catch (err) {
    console.error("Ошибка при экспорте игрушек в файл", err);
    throw err;
  }
}
export default exportToysToExcel;
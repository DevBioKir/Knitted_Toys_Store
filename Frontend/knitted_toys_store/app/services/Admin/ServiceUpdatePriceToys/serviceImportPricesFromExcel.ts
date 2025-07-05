import adminAPI from "../adminAPI";

export async function importPricesFromExcel(excelFile: File) {
  if (!excelFile) throw new Error("Файл не выбран/не верный формат");

  const formData = new FormData();
  formData.append("excelFile", excelFile);

  try {
    await adminAPI.post("/ToysPriceUpdate/ImportPricesFromExcel", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  } catch (err) {
    console.error("Ошибка при импорте изменений в базу данных", err);
    throw err;
  }
};

export default importPricesFromExcel;
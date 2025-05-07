import adminAPI from "./adminAPI";

export async function uploadToysFromExcel(zipFile: File) {
  if (!zipFile) throw new Error("Файл не выбран/не верный формат");

  const formData = new FormData();
  formData.append("zipFile", zipFile);

  try {
    await adminAPI.post("/UploadToysFromExcel/UploadExel", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  } catch (err) {
    console.error("Ошибка при загрузке файла", err);
    throw err;
  }
};

export default uploadToysFromExcel;

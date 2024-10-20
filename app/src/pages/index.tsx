import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@radix-ui/react-label";
import { RadioGroup, RadioGroupItem } from "@radix-ui/react-radio-group";
import { GetServerSidePropsContext } from "next";
import Head from "next/head";
import { useState } from "react";

const Index = ({

} : {

}) => {
    const [tipoPessoa, setTipoPessoa] = useState<'fisica' | 'juridica'>('fisica');

    return (
        <div className="max-w-md mx-auto p-6 bg-white shadow-md rounded-md">
      <h2 className="text-2xl font-bold mb-4">Cadastro de Cliente</h2>

      {/* Tipo de Pessoa */}
      <div className="mb-4">
        <Label className="block font-medium mb-2">Tipo de Pessoa:</Label>
        <RadioGroup 
          className="flex space-x-4"
          value={tipoPessoa}
          onValueChange={(value) => setTipoPessoa(value as 'fisica' | 'juridica')}
        >
          <div className="flex items-center">
            <RadioGroupItem value="fisica" id="fisica" />
            <Label htmlFor="fisica" className="ml-2">Pessoa Física</Label>
          </div>
          <div className="flex items-center">
            <RadioGroupItem value="juridica" id="juridica" />
            <Label htmlFor="juridica" className="ml-2">Pessoa Jurídica</Label>
          </div>
        </RadioGroup>
      </div>

      {/* Nome Fantasia (Só aparece se for Pessoa Jurídica) */}
      {tipoPessoa === 'juridica' && (
        <div className="mb-4">
          <Label htmlFor="nomeFantasia" className="block font-medium mb-2">Nome Fantasia:</Label>
          <Input
            id="nomeFantasia"
            type="text"
            placeholder="Nome Fantasia"
            className="w-full"
          />
        </div>
      )}

      {/* Nome Legal */}
      <div className="mb-4">
        <Label htmlFor="nomeLegal" className="block font-medium mb-2">Nome Legal:</Label>
        <Input
          id="nomeLegal"
          type="text"
          placeholder="Nome Legal"
          className="w-full"
        />
      </div>

      {/* Nº de Documento */}
      <div className="mb-4">
        <Label htmlFor="numeroDocumento" className="block font-medium mb-2">Nº de Documento:</Label>
        <Input
          id="numeroDocumento"
          type="text"
          placeholder="Nº do Documento"
          className="w-full"
        />
      </div>

      {/* Email */}
      <div className="mb-4">
        <Label htmlFor="email" className="block font-medium mb-2">Email:</Label>
        <Input
          id="email"
          type="email"
          placeholder="Email"
          className="w-full"
        />
      </div>

      {/* Telefone */}
      <div className="mb-4">
        <Label className="block font-medium mb-2">Telefone:</Label>
        <div className="flex space-x-2">
          <Input
            type="text"
            placeholder="DDI"
            className="w-1/4"
          />
          <Input
            type="text"
            placeholder="DDD"
            className="w-1/4"
          />
          <Input
            type="text"
            placeholder="Número"
            className="w-1/2"
          />
        </div>
      </div>

      {/* Botão de envio */}
      <div>
        <Button type="submit" className="w-full">
          Enviar
        </Button>
      </div>
    </div>
    );
}

export async function getServerSideProps(context: GetServerSidePropsContext) {
    return {
        props : {

        }
    }
}

export default Index;